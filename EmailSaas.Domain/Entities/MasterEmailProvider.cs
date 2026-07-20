using EmailSaas.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Domain.Entities
{
    public class MasterEmailProvider : AuditableEntity
    {
        public int ClientID { get; set; }
        public string ProviderName { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public string SenderEmail { get; set; } = string.Empty;
        public string? ReplyToEmail { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? SMTPHost { get; set; }
        public int? SMTPPort { get; set; }
        public string? APIKey { get; set; }
        public bool IsDefault { get; set; }
        public string? IMAPHost { get; set; }
        public string? IMPAUserName { get; set; }
        public string? IMAPPassword { get; set; }
        public int? IMAPPort { get; set; }
        public bool IMAPSSL { get; set; } = true;
        public string? APIEndPoint { get; set; }
        public byte Status { get; set; }

        public MasterClient Client { get; set; } = null!;
    }
}
