using EmailSaas.Application.Common.Interfaces;
using EmailSaas.Application.Common.Models;
using EmailSaas.Application.DTOs.Client;
using EmailSaas.Domain.Entities;
using EmailSaas.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.Features.Clients.Commands.CreateClient
{
    public class CreateClientCommandHandler : IRequestHandler<CreateClientCommand, Result<ClientResponseDto>>
    {
        private readonly IApplicationDbContext _context;

        public CreateClientCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<ClientResponseDto>> Handle(CreateClientCommand request, CancellationToken cancellationToken)
        {
            // Check application exists
            var application = await _context.ApplicationMasters
                .FirstOrDefaultAsync(x => x.Id == request.ApplicationId, cancellationToken);

            if (application == null)
                return Result<ClientResponseDto>.Failure($"Application with Id '{request.ApplicationId}' not found.");

            // Check duplicate client code
            var exists = await _context.ClientMasters
                .AnyAsync(x => x.ClientCode == request.ClientCode, cancellationToken);

            if (exists)
                return Result<ClientResponseDto>.Failure($"ClientCode '{request.ClientCode}' already exists.");

            var entity = new ClientMaster
            {
                ApplicationId = request.ApplicationId,
                ClientCode = request.ClientCode,
                ClientName = request.ClientName,
                LogoUrl = request.LogoUrl,
                PrimaryColor = request.PrimaryColor,
                Status = (byte)CommonStatus.Active,
                CreatedBy = request.CreatedBy,
                CreatedDate = DateTime.UtcNow
            };

            _context.ClientMasters.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            var response = new ClientResponseDto
            {
                Id = entity.Id,
                ApplicationId = entity.ApplicationId,
                ApplicationCode = application.ApplicationCode,
                ApplicationName = application.ApplicationName,
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
