using EmailSaas.Application.Common.Interfaces;
using EmailSaas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace EmailSaas.Infrastructure.Services
{
    public class WebhookDispatcher : IWebhookDispatcher
    {
        private readonly IApplicationDbContext _context;

        public WebhookDispatcher(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task QueueWebhookAsync(int emailLogId, string eventType, object eventPayload, CancellationToken cancellationToken)
        {
            // Webhook functionality temporarily commented out
            await Task.CompletedTask;
            /*
            var emailLog = await _context.EmailLogs
                .FirstOrDefaultAsync(x => x.Id == emailLogId, cancellationToken);

            if (emailLog == null)
                return; // nothing to notify about — caller already logged the event itself

            // Find active subscriptions for this client that care about this event type
            var subscriptions = await _context.WebhookSubscriptions
                .Where(x => x.ClientID == emailLog.ClientID
                         && x.Status == 1 // Active
                         && x.EventTypes.Contains(eventType))
                .ToListAsync(cancellationToken);

            if (subscriptions.Count == 0)
                return; // no one is listening for this event — nothing to queue

            var now = DateTime.UtcNow;

            foreach (var sub in subscriptions)
            {
                // Guard: EventTypes is stored as CSV — Contains() above can false-positive on
                // substrings (e.g. "Bounced" contains "Bounce"), so double-check with exact split match
                var subscribedEvents = sub.EventTypes.Split(',', StringSplitOptions.RemoveEmptyEntries);
                if (!subscribedEvents.Contains(eventType, StringComparer.OrdinalIgnoreCase))
                    continue;

                var payload = BuildPayload(emailLog, eventType, eventPayload);

                _context.WebhookDeliveryLogs.Add(new WebhookDeliveryLog
                {
                    WebhookSubscriptionId = sub.Id,
                    LogID = emailLog.Id,
                    EventType = eventType,
                    Payload = payload,
                    AttemptNumber = 1,
                    IsSuccess = false,
                    NextRetryAt = now, // eligible for immediate pickup by background service
                    CreatedBy = "System",
                    CreatedDate = now
                });
            }

            await _context.SaveChangesAsync(cancellationToken);
            */
        }

        private static string BuildPayload(EmailLog emailLog, string eventType, object eventPayload)
        {
            var payload = new
            {
                EventType = eventType,
                MessageID = emailLog.MessageID,
                LogID = emailLog.Id,
                ClientID = emailLog.ClientID,
                ToEmail = emailLog.ToEmail,
                OccurredAt = DateTime.UtcNow,
                Data = eventPayload
            };

            return JsonSerializer.Serialize(payload);
        }
    }
}