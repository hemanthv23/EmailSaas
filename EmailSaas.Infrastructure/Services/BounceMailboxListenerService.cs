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

        private static readonly Regex MessageIdHeaderPattern =
            new(@"X-EmailSaas-MessageId:\s*(MSG-[A-F0-9]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

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

            var configs = await context.EmailProviderConfigs
                .Where(x => x.BounceMonitoringEnabled
                         && x.Status == 1
                         && x.ImapHost != null)
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
                    _logger.LogError(ex, "Failed to process bounce mailbox for ClientId={ClientId}", config.ClientId);
                }
            }
        }

        private async Task ProcessSingleMailboxAsync(
    EmailSaas.Domain.Entities.EmailProviderConfig config,
    IEncryptionService encryptionService,
    IMediator mediator,
    CancellationToken stoppingToken)
        {
            // Use proper field mappings based on your DB schema:
            // UserName = TenantId, ApiKeyEncrypted = ClientId, PasswordEncrypted = ClientSecret
            var tenantId = config.UserName;
            var azureClientId = config.ApiKeyEncrypted ?? string.Empty;
            var clientSecret = !string.IsNullOrEmpty(config.PasswordEncrypted)
                ? encryptionService.Decrypt(config.PasswordEncrypted)
                : string.Empty;

            // Dynamically retrieve the mailbox address from the IMAP configuration field
            var targetMailbox = config.ImapUserName ?? config.SenderEmail;

            if (string.IsNullOrEmpty(tenantId) || string.IsNullOrEmpty(azureClientId) ||
                string.IsNullOrEmpty(clientSecret) || string.IsNullOrEmpty(targetMailbox))
            {
                _logger.LogWarning("ClientId={ClientId}: missing Graph credentials or target mailbox UPN.", config.ClientId);
                return;
            }

            var credential = new ClientSecretCredential(tenantId, azureClientId, clientSecret);

            var tokenRequestContext = new TokenRequestContext(
                new[] { "https://outlook.office365.com/.default" });

            var accessToken = await credential.GetTokenAsync(tokenRequestContext, stoppingToken);

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwt = tokenHandler.ReadJwtToken(accessToken.Token);

            _logger.LogInformation("========== TOKEN CLAIMS ==========");

            foreach (var claim in jwt.Claims)
            {
                _logger.LogInformation("{Type} = {Value}", claim.Type, claim.Value);
            }

            _logger.LogInformation("==================================");

            using var client = new ImapClient(new ProtocolLogger(Console.OpenStandardOutput()));

            await client.ConnectAsync(config.ImapHost, config.ImapPort ?? 993,
                config.ImapUseSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls,
                stoppingToken);

            _logger.LogWarning(
                "IMAP OAuth login. Username={Username}, Host={Host}, Port={Port}",
                targetMailbox,
                config.ImapHost,
                config.ImapPort);

            // Pass the dynamic mailbox UPN and the acquired token to MailKit SASL
            var mailbox = targetMailbox.Trim();

            _logger.LogInformation("Authenticating mailbox [{Mailbox}]", mailbox);

            var oauth2 = new SaslMechanismOAuth2(
                mailbox,
                accessToken.Token.Trim());

            await client.AuthenticateAsync(oauth2, stoppingToken);

            var inbox = client.Inbox;
            await inbox.OpenAsync(FolderAccess.ReadWrite, stoppingToken);

            var unreadIds = await inbox.SearchAsync(SearchQuery.NotSeen, stoppingToken);

            if (unreadIds.Count == 0)
            {
                await client.DisconnectAsync(true, stoppingToken);
                return;
            }

            _logger.LogInformation("ClientId={ClientId}: found {Count} unread bounce candidates.", config.ClientId, unreadIds.Count);

            foreach (var uid in unreadIds)
            {
                var message = await inbox.GetMessageAsync(uid, stoppingToken);

                if (!IsBounceMessage(message))
                {
                    await inbox.AddFlagsAsync(uid, MessageFlags.Seen, true, stoppingToken);
                    continue;
                }

                var originalMessageId = ExtractOriginalMessageId(message);

                if (!string.IsNullOrEmpty(originalMessageId))
                {
                    var result = await mediator.Send(new RecordEmailBouncedCommand
                    {
                        MessageId = originalMessageId,
                        BounceReason = ExtractBounceReason(message),
                        IsHardBounce = DetermineIfHardBounce(message)
                    }, stoppingToken);

                    if (!result.Succeeded)
                        _logger.LogWarning("Bounce mail matched no EmailLog: MessageId={MessageId}", originalMessageId);
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

        private static string? ExtractOriginalMessageId(MimeMessage message)
        {
            foreach (var part in message.BodyParts)
            {
                if (part is MessagePart rfc822 && rfc822.Message != null)
                {
                    var headerValue = rfc822.Message.Headers["X-EmailSaas-MessageId"];
                    if (!string.IsNullOrEmpty(headerValue))
                        return headerValue;
                }
            }

            var fullText = (message.TextBody ?? string.Empty) + " " + (message.HtmlBody ?? string.Empty);
            var match = MessageIdHeaderPattern.Match(fullText);
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