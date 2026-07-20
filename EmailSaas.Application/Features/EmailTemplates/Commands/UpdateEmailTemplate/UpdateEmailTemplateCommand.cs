using EmailSaas.Application.Common.Models;
using EmailSaas.Application.DTOs.EmailTemplate;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.Features.EmailTemplates.Commands.UpdateEmailTemplate
{
    public class UpdateEmailTemplateCommand : IRequest<Result<EmailTemplateResponseDto>>
    {
        public int Id { get; set; }
        public string TemplateName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string SubjectTemplate { get; set; } = string.Empty;
        public string? SubjectVariables { get; set; }
        public string BodyTemplate { get; set; } = string.Empty;
        public string? BodyVariables { get; set; }
        public string? FromEmailOverride { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
    }
}
