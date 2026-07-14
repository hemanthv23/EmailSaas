using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.DTOs.Client
{
    public class ClientRequestDto
    {
        public int ApplicationId { get; set; }
        public string ClientCode { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public string? PrimaryColor { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }
}
