using EmailSaas.Application.Common.Exceptions;
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

namespace EmailSaas.Application.Features.Clients.Queries.GetClientById
{
    public class GetClientByIdQueryHandler : IRequestHandler<GetClientByIdQuery, Result<ClientResponseDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetClientByIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<ClientResponseDto>> Handle(GetClientByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _context.MasterClients
                .AsNoTracking()
                .Include(x => x.Application)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity == null)
                throw new NotFoundException("Client", request.Id);

            var response = new ClientResponseDto
            {
                Id = entity.Id,
                ApplicationId = entity.ApplicationId,
                ApplicationCode = entity.Application.ApplicationCode,
                ApplicationName = entity.Application.ApplicationName,
                ClientCode = entity.ClientCode,
                ClientName = entity.ClientName,
                LogoUrl = entity.LogoUrl,
                PrimaryColor = entity.PrimaryColor,
                Status = entity.Status,
                CreatedBy = entity.CreatedBy,
                CreatedDate = entity.CreatedDate,
                UpdatedBy = entity.UpdatedBy,
                UpdatedDate = entity.UpdatedDate
            };

            return Result<ClientResponseDto>.Success(response);
        }
    }
}
