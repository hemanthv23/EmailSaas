using EmailSaas.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Domain.Entities
{
    public class EmailTemplateMaster : AuditableEntity
    {
        public int ClientId { get; set; }
        public string TemplateCode { get; set; } = string.Empty;
        public string TemplateName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string SubjectTemplate { get; set; } = string.Empty;
        public string? SubjectVariables { get; set; }
        public string BodyTemplate { get; set; } = string.Empty;
        public string? BodyVariables { get; set; }
        public string? FromEmailOverride { get; set; }
        public byte Status { get; set; }

        public ClientMaster Client { get; set; } = null!;
        public ICollection<EmailLog> EmailLogs { get; set; } = new List<EmailLog>();
    }
}
