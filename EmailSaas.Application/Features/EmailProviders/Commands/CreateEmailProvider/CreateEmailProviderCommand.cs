using EmailSaas.Application.Common.Models;
using EmailSaas.Application.DTOs.EmailProvider;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.Features.EmailProviders.Commands.CreateEmailProvider
{
    public class CreateEmailProviderCommand : EmailProviderRequestDto, IRequest<Result<EmailProviderResponseDto>>
    {
    }
}
