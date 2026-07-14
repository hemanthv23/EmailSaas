using EmailSaas.Application.Common.Models;
using EmailSaas.Application.DTOs.SendEmail;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.Features.SendEmail.Commands
{
    public class SendEmailCommand : SendEmailRequestDto, IRequest<Result<SendEmailResponseDto>>
    {
    }
}
