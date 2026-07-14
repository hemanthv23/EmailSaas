using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.Common.Interfaces
{
    public interface IWebhookDispatcher
    {
        /// <summary>
        /// Finds all active WebhookSubscriptions for the client owning the given EmailLog
        /// that are subscribed to eventType, and queues a WebhookDeliveryLog (Pending) for each.
        /// Does NOT perform the actual HTTP call — that's handled by the background dispatch service.
        /// </summary>
        Task QueueWebhookAsync(int emailLogId, string eventType, object eventPayload, CancellationToken cancellationToken);
    }
}
