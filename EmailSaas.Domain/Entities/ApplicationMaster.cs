using EmailSaas.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Domain.Entities
{
    public class ApplicationMaster : AuditableEntity
    {
        public string ApplicationCode { get; set; } = string.Empty;
        public string ApplicationName { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public byte Status { get; set; }
        public ICollection<ClientMaster> Clients { get; set; } = new List<ClientMaster>();
    }
}
