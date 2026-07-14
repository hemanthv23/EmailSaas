using EmailSaas.Application.Common.Interfaces;
using EmailSaas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace EmailSaas.Infrastructure.Services
{
    public class WebhookDispatchBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly WebhookSettings _settings;
        private readonly ILogger<WebhookDispatchBackgroundService> _logger;

        public WebhookDispatchBackgroundService(
            IServiceScopeFactory scopeFactory,
            IHttpClientFactory httpClientFactory,
            IOptions<WebhookSettings> settings,
            ILogger<WebhookDispatchBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _httpClientFactory = httpClientFactory;
            _settings = settings.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("WebhookDispatchBackgroundService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessPendingDeliveriesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    // Never let one bad cycle kill the whole background service
                    _logger.LogError(ex, "Unhandled error in webhook dispatch cycle.");
                }

                await Task.Delay(TimeSpan.FromSeconds(_settings.PollingIntervalSeconds), stoppingToken);
            }
        }

        private async Task ProcessPendingDeliveriesAsync(CancellationToken stoppingToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

            var now = DateTime.UtcNow;

            var pending = await context.WebhookDeliveryLogs
                .Include(x => x.WebhookSubscription)
                .Where(x => !x.IsSuccess
                         && x.NextRetryAt <= now
                         && x.AttemptNumber <= _settings.MaxAttempts
                         && x.WebhookSubscription.Status == 1) // Active subscription only
                .OrderBy(x => x.NextRetryAt)
                .Take(50) // batch size per cycle — avoids overload on huge backlogs
                .ToListAsync(stoppingToken);

            if (pending.Count == 0)
                return;

            _logger.LogInformation("Processing {Count} pending webhook deliveries.", pending.Count);

            var httpClient = _httpClientFactory.CreateClient("WebhookClient");

            foreach (var delivery in pending)
            {
                await DeliverAsync(delivery, httpClient, stoppingToken);
            }

            await context.SaveChangesAsync(stoppingToken);
        }

        private async Task DeliverAsync(WebhookDeliveryLog delivery, HttpClient httpClient, CancellationToken stoppingToken)
        {
            try
            {
                var signature = ComputeSignature(delivery.Payload, delivery.WebhookSubscription.Secret);

                using var httpRequest = new HttpRequestMessage(HttpMethod.Post, delivery.WebhookSubscription.CallbackUrl)
                {
                    Content = new StringContent(delivery.Payload, Encoding.UTF8, "application/json")
                };
                httpRequest.Headers.Add("X-Webhook-Signature", signature);
                httpRequest.Headers.Add("X-Webhook-Event", delivery.EventType);
                httpRequest.Headers.Add("X-Webhook-Attempt", delivery.AttemptNumber.ToString());

                using var cts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                cts.CancelAfter(TimeSpan.FromSeconds(_settings.TimeoutSeconds));

                var response = await httpClient.SendAsync(httpRequest, cts.Token);
                var responseBody = await response.Content.ReadAsStringAsync(stoppingToken);

                delivery.ResponseStatusCode = (int)response.StatusCode;
                delivery.ResponseBody = Truncate(responseBody, 2000);

                if (response.IsSuccessStatusCode)
                {
                    delivery.IsSuccess = true;
                    delivery.DeliveredAt = DateTime.UtcNow;
                    delivery.UpdatedDate = DateTime.UtcNow;
                    _logger.LogInformation("Webhook delivered: DeliveryLogId={Id}, Status={Status}",
                        delivery.Id, delivery.ResponseStatusCode);
                }
                else
                {
                    ScheduleRetry(delivery, $"Non-success status code: {response.StatusCode}");
                }
            }
            catch (TaskCanceledException)
            {
                ScheduleRetry(delivery, "Request timed out.");
            }
            catch (Exception ex)
            {
                ScheduleRetry(delivery, $"Exception: {ex.Message}");
            }
        }

        private void ScheduleRetry(WebhookDeliveryLog delivery, string errorReason)
        {
            delivery.AttemptNumber += 1;
            delivery.UpdatedDate = DateTime.UtcNow;

            if (delivery.AttemptNumber > _settings.MaxAttempts)
            {
                // Give up — stays IsSuccess=false permanently, visible in logs for manual investigation
                delivery.ResponseBody = Truncate($"Max attempts reached. Last error: {errorReason}", 2000);
                _logger.LogWarning("Webhook delivery exhausted retries: DeliveryLogId={Id}, Reason={Reason}",
                    delivery.Id, errorReason);
                return;
            }

            // Exponential backoff: base * 2^(attempt-1) — e.g. 60s, 120s, 240s, 480s...
            var backoffSeconds = _settings.BaseBackoffSeconds * Math.Pow(2, delivery.AttemptNumber - 1);
            delivery.NextRetryAt = DateTime.UtcNow.AddSeconds(backoffSeconds);
            delivery.ResponseBody = Truncate(errorReason, 2000);

            _logger.LogWarning("Webhook delivery failed, retry scheduled: DeliveryLogId={Id}, NextAttempt={Attempt}, NextRetryAt={NextRetryAt}",
                delivery.Id, delivery.AttemptNumber, delivery.NextRetryAt);
        }

        private static string ComputeSignature(string payload, string secret)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            return Convert.ToHexString(hash).ToLowerInvariant();
        }

        private static string Truncate(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value[..maxLength];
        }
    }
}