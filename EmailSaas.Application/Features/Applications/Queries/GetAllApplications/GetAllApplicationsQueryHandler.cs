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

namespace EmailSaas.Application.Features.Applications.Queries.GetAllApplications
{
    public class GetAllApplicationsQueryHandler : IRequestHandler<GetAllApplicationsQuery, Result<List<ApplicationResponseDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllApplicationsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<ApplicationResponseDto>>> Handle(GetAllApplicationsQuery request, CancellationToken cancellationToken)
        {
            var applications = await _context.MasterApplications
                .AsNoTracking()
                .Where(x => x.Id == request.ApplicationId)
                .OrderByDescending(x => x.CreatedDate)
                .Select(x => new ApplicationResponseDto
                {
                    Id = x.Id,
                    ApplicationCode = x.ApplicationCode,
                    ApplicationName = x.ApplicationName,
                    ApiKey = x.ApiKey,
                    Status = x.Status,
                    CreatedBy = x.CreatedBy,
                    CreatedDate = x.CreatedDate,
                    UpdatedBy = x.UpdatedBy,
                    UpdatedDate = x.UpdatedDate
                })
                .ToListAsync(cancellationToken);

            return Result<List<ApplicationResponseDto>>.Success(applications);
        }
    }
}
