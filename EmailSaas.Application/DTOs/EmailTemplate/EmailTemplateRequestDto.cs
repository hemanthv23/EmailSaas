using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.DTOs.EmailTemplate
{
    public class EmailTemplateRequestDto
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
        public string CreatedBy { get; set; } = string.Empty;

    }
}
