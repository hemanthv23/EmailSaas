using EmailSaas.Application.Common.Models;
using EmailSaas.Application.DTOs.Client;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.Features.Clients.Commands.CreateClient
{
    public class CreateClientCommand : ClientRequestDto, IRequest<Result<ClientResponseDto>>
    {
    }
}
