using EmailSaas.Application.Common.Interfaces;
using EmailSaas.Application.Common.Models;
using EmailSaas.Application.DTOs.EmailProvider;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.Features.EmailProviders.Queries.GetAllEmailProviders
{
    public class GetAllEmailProvidersQueryHandler : IRequestHandler<GetAllEmailProvidersQuery, Result<List<EmailProviderResponseDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllEmailProvidersQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<EmailProviderResponseDto>>> Handle(GetAllEmailProvidersQuery request, CancellationToken cancellationToken)
        {
            var providers = await _context.MasterEmailProviders
                .AsNoTracking()
                .Include(x => x.Client)
                .Where(x => x.Client.ApplicationId == request.ApplicationId)
                .OrderByDescending(x => x.CreatedDate)
                .Select(x => new EmailProviderResponseDto
                {
                    Id = x.Id,
                    ClientID = x.ClientID,
                    ClientCode = x.Client.ClientCode,
                    ClientName = x.Client.ClientName,
                    ProviderName = x.ProviderName,
                    SenderName = x.SenderName,
                    SenderEmail = x.SenderEmail,
                    ReplyToEmail = x.ReplyToEmail,
                    SMTPHost = x.SMTPHost,
                    SMTPPort = x.SMTPPort,
                    UserName = x.UserName,
                    IsDefault = x.IsDefault,
                    Status = x.Status,
                    CreatedBy = x.CreatedBy,
                    CreatedDate = x.CreatedDate,
                    UpdatedBy = x.UpdatedBy,
                    UpdatedDate = x.UpdatedDate
                })
                .ToListAsync(cancellationToken);

            return Result<List<EmailProviderResponseDto>>.Success(providers);
        }
    }
}
