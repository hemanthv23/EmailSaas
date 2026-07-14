using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.DTOs.SendEmail
{
    public class SendEmailRequestDto
    {
        public int ApplicationId { get; set; }
        public int ClientId { get; set; }
        public string TemplateCode { get; set; } = string.Empty;
        public string ToEmail { get; set; } = string.Empty;
        public string? CcEmail { get; set; }
        public string? BccEmail { get; set; }
        public Dictionary<string, string> Parameters { get; set; } = new();
        public string CreatedBy { get; set; } = string.Empty;
    }
}
