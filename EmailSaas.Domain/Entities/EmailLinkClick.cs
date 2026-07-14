using EmailSaas.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Domain.Entities
{
    public class EmailLinkClick : AuditableEntity
    {
        public int EmailLogId { get; set; }
        public string OriginalUrl { get; set; } = string.Empty;
        public DateTime ClickedAt { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }

        public EmailLog EmailLog { get; set; } = null!;
    }
}
