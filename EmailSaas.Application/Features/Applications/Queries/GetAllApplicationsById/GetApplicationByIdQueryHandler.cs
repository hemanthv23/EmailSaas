using EmailSaas.Application.Common.Exceptions;
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

namespace EmailSaas.Application.Features.Applications.Queries.GetAllApplicationsById
{
    public class GetApplicationByIdQueryHandler : IRequestHandler<GetApplicationByIdQuery, Result<ApplicationResponseDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetApplicationByIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<ApplicationResponseDto>> Handle(GetApplicationByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _context.ApplicationMasters
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity == null)
                throw new NotFoundException("Application", request.Id);

            var response = new ApplicationResponseDto
            {
                Id = entity.Id,
                ApplicationCode = entity.ApplicationCode,
                ApplicationName = entity.ApplicationName,
                ApiKey = entity.ApiKey,
                Status = entity.Status,
                CreatedBy = entity.CreatedBy,
                CreatedDate = entity.CreatedDate,
                UpdatedBy = entity.UpdatedBy,
                UpdatedDate = entity.UpdatedDate
            };

            return Result<ApplicationResponseDto>.Success(response);
        }
    }
}
