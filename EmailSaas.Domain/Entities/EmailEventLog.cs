using EmailSaas.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Domain.Entities
{
    public class EmailEvent : AuditableEntity
    {
        public int EmailLogId { get; set; }
        public string EventType { get; set; } = string.Empty;     // Delivered / Opened / Clicked / Bounced / Failed
        public string? EventData { get; set; }                    // raw JSON payload (IP, UA, provider response, etc.)
        public DateTime OccurredAt { get; set; }

        public EmailLog EmailLog { get; set; } = null!;
    }
}
