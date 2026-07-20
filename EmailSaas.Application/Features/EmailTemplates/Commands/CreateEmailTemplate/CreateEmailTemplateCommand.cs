using EmailSaas.Application.Common.Models;
using EmailSaas.Application.DTOs.EmailTemplate;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.Features.EmailTemplates.Commands.CreateEmailTemplate
{
    public class CreateEmailTemplateCommand : EmailTemplateRequestDto, IRequest<Result<EmailTemplateResponseDto>>
    {
    }
}
