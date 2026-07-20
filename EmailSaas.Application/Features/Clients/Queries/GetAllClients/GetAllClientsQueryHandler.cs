using EmailSaas.Application.Common.Interfaces;
using EmailSaas.Application.Common.Models;
using EmailSaas.Application.DTOs.Client;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.Features.Clients.Queries.GetAllClients
{
    public class GetAllClientsQueryHandler : IRequestHandler<GetAllClientsQuery, Result<List<ClientResponseDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllClientsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<ClientResponseDto>>> Handle(GetAllClientsQuery request, CancellationToken cancellationToken)
        {
            var clients = await _context.MasterClients
                .AsNoTracking()
                .Include(x => x.Application)
                 .Where(x => x.ApplicationId == request.ApplicationId)
                .OrderByDescending(x => x.CreatedDate)
                .Select(x => new ClientResponseDto
                {
                    Id = x.Id,
                    ApplicationId = x.ApplicationId,
                    ApplicationCode = x.Application.ApplicationCode,
                    ApplicationName = x.Application.ApplicationName,
                    ClientCode = x.ClientCode,
                    ClientName = x.ClientName,
                    LogoUrl = x.LogoUrl,
                    PrimaryColor = x.PrimaryColor,
                    Status = x.Status,
                    CreatedBy = x.CreatedBy,
                    CreatedDate = x.CreatedDate,
                    UpdatedBy = x.UpdatedBy,
                    UpdatedDate = x.UpdatedDate
                })
                .ToListAsync(cancellationToken);

            return Result<List<ClientResponseDto>>.Success(clients);
        }
    }
}
