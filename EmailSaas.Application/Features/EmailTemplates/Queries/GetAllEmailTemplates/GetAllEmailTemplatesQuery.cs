using EmailSaas.Application.Common.Models;
using EmailSaas.Application.DTOs.EmailTemplate;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.Features.EmailTemplates.Queries.GetAllEmailTemplates
{
    public class GetAllEmailTemplatesQuery : IRequest<Result<List<EmailTemplateResponseDto>>>
    {
        public int ApplicationId { get; set; }
    }
}
