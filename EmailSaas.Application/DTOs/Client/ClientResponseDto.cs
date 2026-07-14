using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.DTOs.Client
{
    public class ClientResponseDto
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public string ApplicationCode { get; set; } = string.Empty;
        public string ApplicationName { get; set; } = string.Empty;
        public string ClientCode { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public string? PrimaryColor { get; set; }
        public byte Status { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
