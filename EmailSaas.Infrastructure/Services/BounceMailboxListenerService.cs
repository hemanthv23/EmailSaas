using Azure.Core;
using Azure.Identity;
using EmailSaas.Application.Common.Interfaces;
using EmailSaas.Application.Features.Tracking.Commands.RecordEmailBounced;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MimeKit;
using System.Text.RegularExpressions;
using System.IdentityModel.Tokens.Jwt;

namespace EmailSaas.Infrastructure.Services
{
    public class BounceMailboxListenerService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<BounceMailboxListenerService> _logger;
        private const int PollingIntervalSeconds = 120;

        private static readonly Regex MessageIDHeaderPattern =
            new(@"X-EmailSaas-MessageID:\s*(MSG-[A-F0-9]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public BounceMailboxListenerService(
            IServiceScopeFactory scopeFactory,
            ILogger<BounceMailboxListenerService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("BounceMailboxListenerService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessAllClientMailboxesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in bounce mailbox polling cycle.");
                }

                await Task.Delay(TimeSpan.FromSeconds(PollingIntervalSeconds), stoppingToken);
            }
        }

        private async Task ProcessAllClientMailboxesAsync(CancellationToken stoppingToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
            var encryptionService = scope.ServiceProvider.GetRequiredService<IEncryptionService>();

            var configs = await context.MasterEmailProviders
                .Where(x => x.Status == 1 && x.IMAPHost != null)
                .ToListAsync(stoppingToken);

            if (configs.Count == 0)
                return;

            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            foreach (var config in configs)
            {
                try
                {
                    await ProcessSingleMailboxAsync(config, encryptionService, mediator, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process bounce mailbox for ClientID={ClientID}", config.ClientID);
                }
            }
        }

        private async Task ProcessSingleMailboxAsync(
    EmailSaas.Domain.Entities.MasterEmailProvider config,
    IEncryptionService encryptionService,
    IMediator mediator,
    CancellationToken stoppingToken)
        {
            var targetMailbox = config.IMPAUserName ?? config.SenderEmail;
            var isGraphApi = config.ProviderName?.Contains("Graph", StringComparison.OrdinalIgnoreCase) == true || 
                             config.ProviderName?.Contains("Microsoft", StringComparison.OrdinalIgnoreCase) == true;

            using var client = new ImapClient(new ProtocolLogger(Console.OpenStandardOutput()));

            await client.ConnectAsync(config.IMAPHost, config.IMAPPort ?? 993,
                config.IMAPSSL ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls,
                stoppingToken);

            _logger.LogWarning(
                "IMAP login. Provider={Provider}, Username={Username}, Host={Host}, Port={Port}",
                config.ProviderName,
                targetMailbox,
                config.IMAPHost,
                config.IMAPPort);

            if (isGraphApi)
            {
                var tenantId = config.UserName;
                var azureClientID = config.APIKey ?? string.Empty;
                var clientSecret = !string.IsNullOrEmpty(config.Password)
                    ? encryptionService.Decrypt(config.Password)
                    : string.Empty;

                if (string.IsNullOrEmpty(tenantId) || string.IsNullOrEmpty(azureClientID) ||
                    string.IsNullOrEmpty(clientSecret) || string.IsNullOrEmpty(targetMailbox))
                {
                    _logger.LogWarning("ClientID={ClientID}: missing Graph credentials or target mailbox UPN.", config.ClientID);
                    return;
                }

                var credential = new ClientSecretCredential(tenantId, azureClientID, clientSecret);
                var tokenRequestContext = new TokenRequestContext(new[] { "https://outlook.office365.com/.default" });
                var accessToken = await credential.GetTokenAsync(tokenRequestContext, stoppingToken);

                var mailbox = targetMailbox.Trim();
                _logger.LogInformation("Authenticating mailbox [{Mailbox}] via OAuth2", mailbox);

                var oauth2 = new SaslMechanismOAuth2(mailbox, accessToken.Token.Trim());
                await client.AuthenticateAsync(oauth2, stoppingToken);
            }
            else
            {
                var imapUser = config.IMPAUserName ?? config.UserName ?? config.SenderEmail;
                var imapPassword = !string.IsNullOrEmpty(config.IMAPPassword)
                    ? encryptionService.Decrypt(config.IMAPPassword)
                    : (!string.IsNullOrEmpty(config.Password) 
                        ? encryptionService.Decrypt(config.Password) 
                        : string.Empty);

                if (string.IsNullOrEmpty(imapUser) || string.IsNullOrEmpty(imapPassword))
                {
                    _logger.LogWarning("ClientID={ClientID}: missing IMAP credentials for basic auth.", config.ClientID);
                    return;
                }

                _logger.LogInformation("Authenticating mailbox [{Mailbox}] via Basic Auth", imapUser);
                await client.AuthenticateAsync(imapUser, imapPassword, stoppingToken);
            }

            var inbox = client.Inbox;
            await inbox.OpenAsync(FolderAccess.ReadWrite, stoppingToken);

            var unreadIds = await inbox.SearchAsync(SearchQuery.NotSeen, stoppingToken);

            if (unreadIds.Count == 0)
            {
                await client.DisconnectAsync(true, stoppingToken);
                return;
            }

            _logger.LogInformation("ClientID={ClientID}: found {Count} unread bounce candidates.", config.ClientID, unreadIds.Count);

            foreach (var uid in unreadIds)
            {
                var message = await inbox.GetMessageAsync(uid, stoppingToken);

                if (!IsBounceMessage(message))
                {
                    _logger.LogInformation("ClientID={ClientID}: Message {Uid} skipped. Not identified as a bounce message. Subject: '{Subject}'", config.ClientID, uid, message.Subject);
                    await inbox.AddFlagsAsync(uid, MessageFlags.Seen, true, stoppingToken);
                    continue;
                }

                var originalMessageID = ExtractOriginalMessageID(message);

                if (!string.IsNullOrEmpty(originalMessageID))
                {
                    var result = await mediator.Send(new RecordEmailBouncedCommand
                    {
                        MessageID = originalMessageID,
                        BounceReason = ExtractBounceReason(message),
                        IsHardBounce = DetermineIfHardBounce(message)
                    }, stoppingToken);

                    if (!result.Succeeded)
                        _logger.LogWarning("Bounce mail matched no EmailLog: MessageID={MessageID}", originalMessageID);
                }
                else
                {
                    _logger.LogWarning("ClientID={ClientID}: Message {Uid} identified as bounce, but could not extract X-EmailSaas-MessageID.", config.ClientID, uid);
                }

                await inbox.AddFlagsAsync(uid, MessageFlags.Seen, true, stoppingToken);
            }

            await client.DisconnectAsync(true, stoppingToken);
        }

        private static bool IsBounceMessage(MimeMessage message)
        {
            var subject = message.Subject ?? string.Empty;
            return subject.Contains("Undeliverable", StringComparison.OrdinalIgnoreCase)
                || subject.Contains("Delivery Status Notification", StringComparison.OrdinalIgnoreCase)
                || subject.Contains("Mail Delivery Failed", StringComparison.OrdinalIgnoreCase)
                || subject.Contains("failure notice", StringComparison.OrdinalIgnoreCase)
                || message.Headers.Contains("X-Failed-Recipients");
        }

        private static string? ExtractOriginalMessageID(MimeMessage message)
        {
            foreach (var part in message.BodyParts)
            {
                if (part is MessagePart rfc822 && rfc822.Message != null)
                {
                    var headerValue = rfc822.Message.Headers["X-EmailSaas-MessageID"];
                    if (!string.IsNullOrEmpty(headerValue))
                        return headerValue;
                }
            }

            var fullText = (message.TextBody ?? string.Empty) + " " + (message.HtmlBody ?? string.Empty);
            var match = MessageIDHeaderPattern.Match(fullText);
            return match.Success ? match.Groups[1].Value : null;
        }

        private static string ExtractBounceReason(MimeMessage message)
        {
            var text = message.TextBody ?? message.HtmlBody ?? message.Subject ?? "Unknown bounce reason";
            return text.Length > 1000 ? text[..1000] : text;
        }

        private static bool DetermineIfHardBounce(MimeMessage message)
        {
            var content = (message.TextBody ?? string.Empty) + (message.Subject ?? string.Empty);
            return content.Contains("5.1.1") || content.Contains("5.1.0")
                || content.Contains("user unknown", StringComparison.OrdinalIgnoreCase)
                || content.Contains("does not exist", StringComparison.OrdinalIgnoreCase)
                || content.Contains("no such user", StringComparison.OrdinalIgnoreCase);
        }
    }
}