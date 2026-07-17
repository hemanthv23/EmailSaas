using Azure.Identity;
using EmailSaas.Application.Common.Interfaces;
using EmailSaas.Domain.Entities;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using MimeKit;
using MimeKit.Utils;
using GraphEmailAddress = Microsoft.Graph.Models.EmailAddress;

namespace EmailSaas.Infrastructure.Services;

public class EmailSenderService : IEmailSenderService
{
    private readonly IEncryptionService _encryptionService;

    public EmailSenderService(IEncryptionService encryptionService)
    {
        _encryptionService = encryptionService;
    }

    public async Task<(bool Success, string? ErrorMessage)> SendAsync(
    EmailProviderConfig providerConfig,
    string toEmail,
    string? ccEmail,
    string? bccEmail,
    string subject,
    string htmlBody,
    Dictionary<string, string>? customHeaders = null,
    CancellationToken cancellationToken = default)
    {
        try
        {
            // Microsoft Graph
            if (providerConfig.ProviderName == EmailSaas.Domain.Constants.EmailProviderConstants.MicrosoftGraph)
            {
                return await SendViaMicrosoftGraphAsync(
                    providerConfig,
                    toEmail,
                    ccEmail,
                    bccEmail,
                    subject,
                    htmlBody,
                    customHeaders,
                    cancellationToken);
            }

            var hasSmtpConfig = !string.IsNullOrEmpty(providerConfig.SmtpHost);
            var hasApiKeyConfig = !string.IsNullOrEmpty(providerConfig.ApiKeyEncrypted);

            if (hasSmtpConfig)
            {
                return await SendViaSmtpAsync(
                    providerConfig,
                    toEmail,
                    ccEmail,
                    bccEmail,
                    subject,
                    htmlBody,
                    customHeaders,
                    cancellationToken);
            }

            if (hasApiKeyConfig)
            {
                return await SendViaApiProviderAsync(
                    providerConfig,
                    toEmail,
                    ccEmail,
                    bccEmail,
                    subject,
                    htmlBody,
                    customHeaders,
                    cancellationToken);
            }

            return (false,
                $"Provider '{providerConfig.ProviderName}' has no valid SMTP, API key, or Graph configuration.");
        }
        catch (Exception ex)
        {
            return (false, ex.ToString());
        }
    }


    #region SendViaMicrosoftGraphAsync
    // ─── Microsoft Graph API ───────────────────────────────────
    private async Task<(bool Success, string? ErrorMessage)> SendViaMicrosoftGraphAsync(
        EmailProviderConfig config,
        string toEmail,
        string? ccEmail,
        string? bccEmail,
        string subject,
        string htmlBody,
        Dictionary<string, string>? customHeaders,
        CancellationToken cancellationToken)
    {
        try
        {
            // TenantId reuses UserName column
            var tenantId = config.UserName;

            var azureClientId = config.ApiKeyEncrypted ?? string.Empty;

            // Client Secret (Encrypted)
            var clientSecret = !string.IsNullOrEmpty(config.PasswordEncrypted)
                ? _encryptionService.Decrypt(config.PasswordEncrypted)
                : string.Empty;

            if (string.IsNullOrEmpty(tenantId)
                || string.IsNullOrEmpty(azureClientId)
                || string.IsNullOrEmpty(clientSecret))
            {
                return (false,
                    "Microsoft Graph requires TenantId (UserName), AzureClientId (ApiKey) and ClientSecret (Password).");
            }



            // Get OAuth2 token using Client Credentials flow
            var credential = new ClientSecretCredential(
                tenantId, azureClientId, clientSecret);

            var graphClient = new GraphServiceClient(credential);

            var message = new Message
            {
                Subject = subject,
                IsDeliveryReceiptRequested = true,
                IsReadReceiptRequested = true,
                Body = new ItemBody
                {
                    ContentType = BodyType.Html,
                    Content = htmlBody
                },
                ToRecipients = new List<Recipient>
                {
                    new Recipient
                    {
                        EmailAddress = new GraphEmailAddress { Address = toEmail }
                    }
                }
            };

            if (!string.IsNullOrEmpty(ccEmail))
            {
                message.CcRecipients = new List<Recipient>
                {
                    new Recipient
                    {
                        EmailAddress = new GraphEmailAddress { Address = ccEmail }
                    }
                };
            }

            if (!string.IsNullOrEmpty(bccEmail))
            {
                message.BccRecipients = new List<Recipient>
                {
                    new Recipient
                    {
                        EmailAddress = new GraphEmailAddress { Address = bccEmail }
                    }
                };
            }

            // ─── Custom headers (e.g. X-EmailSaas-MessageId for bounce matching) ───
            if (customHeaders != null && customHeaders.Count > 0)
            {
                message.InternetMessageHeaders = customHeaders
                    .Select(h => new InternetMessageHeader
                    {
                        Name = h.Key,
                        Value = h.Value
                    })
                    .ToList();
            }

            // Send from the configured mailbox (SenderEmail)
            await graphClient.Users[config.SenderEmail]
                .SendMail
                .PostAsync(new Microsoft.Graph.Users.Item.SendMail.SendMailPostRequestBody
                {
                    Message = message,
                    SaveToSentItems = true
                }, cancellationToken: cancellationToken);

            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, ex.ToString());
        }
    }

    #endregion

    #region SendViaSmtpAsync
    // ─── SMTP (works for ANY SMTP-based provider) ─────────────
    private async Task<(bool Success, string? ErrorMessage)> SendViaSmtpAsync(
        EmailProviderConfig config,
        string toEmail,
        string? ccEmail,
        string? bccEmail,
        string subject,
        string htmlBody,
        Dictionary<string, string>? customHeaders,
        CancellationToken cancellationToken)
    {
        try
        {
            var decryptedPassword = !string.IsNullOrEmpty(config.PasswordEncrypted)
                ? _encryptionService.Decrypt(config.PasswordEncrypted)
                : string.Empty;

            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(config.SenderName, config.SenderEmail));
            message.To.Add(MailboxAddress.Parse(toEmail));

            if (!string.IsNullOrEmpty(ccEmail))
                message.Cc.Add(MailboxAddress.Parse(ccEmail));

            if (!string.IsNullOrEmpty(bccEmail))
                message.Bcc.Add(MailboxAddress.Parse(bccEmail));

            if (!string.IsNullOrEmpty(config.ReplyToEmail))
                message.ReplyTo.Add(MailboxAddress.Parse(config.ReplyToEmail));

            message.Subject = subject;
            message.MessageId = MimeUtils.GenerateMessageId();
            message.Date = DateTimeOffset.UtcNow;
            message.Priority = MessagePriority.Normal;
            message.Importance = MessageImportance.Normal;
            message.XPriority = XMessagePriority.Normal;

            message.Headers.Add("X-Mailer", "EmailSaaS v1.0");
            message.Headers.Add("X-EmailSaaS-Version", "1.0");
            message.Headers.Add("List-Unsubscribe",
                $"<mailto:unsubscribe@{GetDomain(config.SenderEmail)}>");
            message.Headers.Add("List-Unsubscribe-Post",
                "List-Unsubscribe=One-Click");

            // Delivery Status Notification (DSN) and Read Receipt headers
            message.Headers.Add("Disposition-Notification-To", config.SenderEmail);
            message.Headers.Add("Return-Receipt-To", config.SenderEmail);

            // ─── Custom headers (e.g. X-EmailSaas-MessageId for bounce matching) ───
            if (customHeaders != null)
            {
                foreach (var header in customHeaders)
                    message.Headers.Add(header.Key, header.Value);
            }

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = htmlBody,
                TextBody = HtmlToPlainText(htmlBody)
            };
            message.Body = bodyBuilder.ToMessageBody();

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(
                config.SmtpHost,
                config.SmtpPort ?? 587,
                SecureSocketOptions.StartTls,
                cancellationToken);

            await smtp.AuthenticateAsync(
                config.UserName,
                decryptedPassword,
                cancellationToken);

            await smtp.SendAsync(message, cancellationToken);
            await smtp.DisconnectAsync(true, cancellationToken);

            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    #endregion

    #region SendViaApiProviderAsync
    // ─── Generic API-key based provider ──
    // (works with a generic REST email API that uses an API key)
    private async Task<(bool Success, string? ErrorMessage)> SendViaApiProviderAsync(
        EmailProviderConfig config,
        string toEmail,
        string? ccEmail,
        string? bccEmail,
        string subject,
        string htmlBody,
        Dictionary<string, string>? customHeaders,
        CancellationToken cancellationToken)
    {
        try
        {
            var decryptedApiKey = !string.IsNullOrEmpty(config.ApiKeyEncrypted)
                ? _encryptionService.Decrypt(config.ApiKeyEncrypted)
                : string.Empty;

            if (string.IsNullOrEmpty(decryptedApiKey))
                return (false, "API key is missing or invalid.");

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", decryptedApiKey);

            var headersWithDsn = customHeaders ?? new Dictionary<string, string>();
            headersWithDsn["Disposition-Notification-To"] = config.SenderEmail;
            headersWithDsn["Return-Receipt-To"] = config.SenderEmail;

            var payload = new
            {
                from = new { email = config.SenderEmail, name = config.SenderName },
                to = new[] { new { email = toEmail } },
                subject = subject,
                html = htmlBody,
                text = HtmlToPlainText(htmlBody),
                headers = headersWithDsn
            };

            var jsonContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json");
            
            // Use the API Endpoint provided in the configuration, fallback to the hardcoded dummy one if not provided
            var apiEndpoint = !string.IsNullOrWhiteSpace(config.ApiEndpoint) 
                ? config.ApiEndpoint 
                : "https://api.email-provider.example.com/v1/send";

            var response = await httpClient.PostAsync(apiEndpoint, jsonContent, cancellationToken);

            if (response.IsSuccessStatusCode)
                return (true, null);

            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            return (false, $"Provider error: {response.StatusCode} - {errorBody}");
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }
    #endregion

    // ─── Helpers ─────────────────────────────────────────────
    private static string GetDomain(string email)
    {
        var parts = email.Split('@');
        return parts.Length == 2 ? parts[1] : "emailsaas.com";
    }

    private static string HtmlToPlainText(string html)
    {
        var text = System.Text.RegularExpressions.Regex
            .Replace(html, "<.*?>", " ");
        text = System.Text.RegularExpressions.Regex
            .Replace(text, @"\s+", " ");
        return text.Trim();
    }
}