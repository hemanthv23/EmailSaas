using EmailSaas.Application.Common.Exceptions;
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

namespace EmailSaas.Application.Features.EmailProviders.Queries.GetEmailProviderById
{
    public class GetEmailProviderByIdQueryHandler : IRequestHandler<GetEmailProviderByIdQuery, Result<EmailProviderResponseDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetEmailProviderByIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<EmailProviderResponseDto>> Handle(GetEmailProviderByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _context.EmailProviderConfigs
                .AsNoTracking()
                .Include(x => x.Client)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity == null)
                throw new NotFoundException("EmailProviderConfig", request.Id);

            var response = new EmailProviderResponseDto
            {
                Id = entity.Id,
                ClientId = entity.ClientId,
                ClientCode = entity.Client.ClientCode,
                ClientName = entity.Client.ClientName,
                ProviderName = entity.ProviderName,
                SenderName = entity.SenderName,
                SenderEmail = entity.SenderEmail,
                ReplyToEmail = entity.ReplyToEmail,
                SmtpHost = entity.SmtpHost,
                SmtpPort = entity.SmtpPort,
                UserName = entity.UserName,
                IsDefault = entity.IsDefault,
                Status = entity.Status,
                CreatedBy = entity.CreatedBy,
                CreatedDate = entity.CreatedDate,
                UpdatedBy = entity.UpdatedBy,
                UpdatedDate = entity.UpdatedDate
            };

            return Result<EmailProviderResponseDto>.Success(response);
        }
    }
}
