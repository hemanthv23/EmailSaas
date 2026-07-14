using MediatR;
using EmailSaas.Application.Common.Models;
using EmailSaas.Application.DTOs.Client;

namespace EmailSaas.Application.Features.Clients.Queries.GetAllClients;

public class GetAllClientsQuery : IRequest<Result<List<ClientResponseDto>>>
{
    public int ApplicationId { get; set; } // set by controller from ApiKey
}