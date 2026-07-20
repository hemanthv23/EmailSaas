using EmailSaas.Application.Common.Interfaces;
using EmailSaas.Application.Common.Models;
using EmailSaas.Application.DTOs.Application;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.Features.Applications.Commands.RegenerateApiKey
{
    public class RegenerateApiKeyCommandHandler : IRequestHandler<RegenerateApiKeyCommand, Result<ApplicationResponseDto>>
    {
        private readonly IApplicationDbContext _context;

        public RegenerateApiKeyCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<ApplicationResponseDto>> Handle(
            RegenerateApiKeyCommand request,
            CancellationToken cancellationToken)
        {
            // Step 1: Find application by ApplicationCode
            var entity = await _context.MasterApplications
                .FirstOrDefaultAsync(x => x.ApplicationCode == request.ApplicationCode,
                    cancellationToken);

            if (entity == null)
                return Result<ApplicationResponseDto>.Failure(
                    $"Application with code '{request.ApplicationCode}' not found.");

            // Step 2: Check application is active
            if (entity.Status == 0)
                return Result<ApplicationResponseDto>.Failure(
                    $"Application '{request.ApplicationCode}' is inactive. Cannot regenerate ApiKey.");

            // Step 3: Store old key for logging
            var oldApiKey = entity.ApiKey;

            // Step 4: Generate brand new ApiKey
            entity.ApiKey = GenerateApiKey();
            entity.UpdatedBy = request.UpdatedBy;
            entity.UpdatedDate = DateTime.UtcNow;

            // Step 5: Save to DB — old key immediately invalidated
            await _context.SaveChangesAsync(cancellationToken);

            // Step 6: Return new ApiKey in response
            var response = new ApplicationResponseDto
            {
                Id = entity.Id,
                ApplicationCode = entity.ApplicationCode,
                ApplicationName = entity.ApplicationName,
                ApiKey = entity.ApiKey, // ← new key
                Status = entity.Status,
                CreatedBy = entity.CreatedBy,
                CreatedDate = entity.CreatedDate,
                UpdatedBy = entity.UpdatedBy,
                UpdatedDate = entity.UpdatedDate
            };

            return Result<ApplicationResponseDto>.Success(response);
        }

        private static string GenerateApiKey()
        {
            return $"ESAAS-{Guid.NewGuid():N}{Guid.NewGuid():N}".ToUpper()[..64];
        }
    }
}
