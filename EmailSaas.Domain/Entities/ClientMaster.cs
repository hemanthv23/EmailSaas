using EmailSaas.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Domain.Entities
{
    public class ClientMaster : AuditableEntity
    {
        public int ApplicationId { get; set; }
        public string ClientCode { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public string? PrimaryColor { get; set; }
        public byte Status { get; set; }

        public ApplicationMaster Application { get; set; } = null!;
        public ICollection<EmailProviderConfig> EmailProviderConfigs { get; set; } = new List<EmailProviderConfig>();
        public ICollection<EmailTemplateMaster> EmailTemplates { get; set; } = new List<EmailTemplateMaster>();
    }
}
