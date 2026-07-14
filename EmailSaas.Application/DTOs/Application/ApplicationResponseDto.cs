using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.DTOs.Application
{
    public class ApplicationResponseDto
    {
        public int Id { get; set; }
        public string ApplicationCode { get; set; } = string.Empty;
        public string ApplicationName { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public byte Status { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
}
