using EmailSaas.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Domain.Entities
{
    public class MasterClient : AuditableEntity
    {
        public int ApplicationId { get; set; }
        public string ClientCode { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public string? PrimaryColor { get; set; }
        public byte Status { get; set; }

        public MasterApplication Application { get; set; } = null!;
        public ICollection<MasterEmailProvider> MasterEmailProviders { get; set; } = new List<MasterEmailProvider>();
        public ICollection<MasterEmailTemplate> EmailTemplates { get; set; } = new List<MasterEmailTemplate>();
    }
}
