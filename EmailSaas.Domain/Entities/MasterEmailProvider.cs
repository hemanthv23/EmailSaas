using EmailSaas.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Domain.Entities
{
    public class EmailProviderConfig : AuditableEntity
    {
        public int ClientId { get; set; }
        public string ProviderName { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public string SenderEmail { get; set; } = string.Empty;
        public string? ReplyToEmail { get; set; }
        public string? SmtpHost { get; set; }
        public int? SmtpPort { get; set; }
        public string? UserName { get; set; }
        public string? PasswordEncrypted { get; set; }
        public string? ApiKeyEncrypted { get; set; }
        public string? ApiEndpoint { get; set; }

        // ─── Bounce mailbox (IMAP) — for SMTP-based providers only ────
        public string? ImapHost { get; set; }
        public int? ImapPort { get; set; }
        public string? ImapUserName { get; set; }
        public string? ImapPasswordEncrypted { get; set; }
        public bool ImapUseSsl { get; set; } = true;
        public bool BounceMonitoringEnabled { get; set; } = false;
        public bool IsDefault { get; set; }
        public byte Status { get; set; }

        public ClientMaster Client { get; set; } = null!;
    }
}
