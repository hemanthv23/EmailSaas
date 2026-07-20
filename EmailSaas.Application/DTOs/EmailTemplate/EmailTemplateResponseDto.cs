using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.DTOs.EmailTemplate
{
    public class EmailTemplateResponseDto
    {
        public int Id { get; set; }
        public int ClientID { get; set; }
        public string ClientCode { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public string TemplateCode { get; set; } = string.Empty;
        public string TemplateName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string SubjectTemplate { get; set; } = string.Empty;
        public string? SubjectVariables { get; set; }
        public string BodyTemplate { get; set; } = string.Empty;
        public string? BodyVariables { get; set; }
        public string? FromEmailOverride { get; set; }
        public byte Status { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
