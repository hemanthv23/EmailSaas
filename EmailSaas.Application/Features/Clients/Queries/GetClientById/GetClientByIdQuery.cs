using EmailSaas.Application.Common.Models;
using EmailSaas.Application.DTOs.Client;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.Features.Clients.Queries.GetClientById
{
    public class GetClientByIdQuery : IRequest<Result<ClientResponseDto>>
    {
        public int Id { get; set; }
    }
}
