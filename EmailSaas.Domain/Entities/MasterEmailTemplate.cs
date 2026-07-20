using EmailSaas.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Domain.Entities
{
    public class MasterEmailTemplate : AuditableEntity
    {
        public int ClientID { get; set; }
        public int ApplicationId { get; set; }
        public string TemplateCode { get; set; } = string.Empty;
        public string TemplateName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string SubjectTemplate { get; set; } = string.Empty;
        public string? SubjectVariables { get; set; }
        public string BodyTemplate { get; set; } = string.Empty;
        public string? BodyVariables { get; set; }
        public string? FromEmailOverride { get; set; }
        public byte Status { get; set; }

        public MasterClient Client { get; set; } = null!;
        public MasterApplication Application { get; set; } = null!;
        public ICollection<EmailLog> EmailLogs { get; set; } = new List<EmailLog>();
    }
}
