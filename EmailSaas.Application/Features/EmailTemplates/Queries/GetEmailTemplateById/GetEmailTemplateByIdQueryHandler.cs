using EmailSaas.Application.Common.Exceptions;
using EmailSaas.Application.Common.Interfaces;
using EmailSaas.Application.Common.Models;
using EmailSaas.Application.DTOs.EmailTemplate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.Features.EmailTemplates.Queries.GetEmailTemplateById
{
    public class GetEmailTemplateByIdQueryHandler : IRequestHandler<GetEmailTemplateByIdQuery, Result<EmailTemplateResponseDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetEmailTemplateByIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<EmailTemplateResponseDto>> Handle(GetEmailTemplateByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _context.MasterEmailTemplates
                .AsNoTracking()
                .Include(x => x.Client)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity == null)
                throw new NotFoundException("MasterEmailTemplate", request.Id);

            var response = new EmailTemplateResponseDto
            {
                Id = entity.Id,
                ClientID = entity.ClientID,
                ClientCode = entity.Client.ClientCode,
                ClientName = entity.Client.ClientName,
                TemplateCode = entity.TemplateCode,
                TemplateName = entity.TemplateName,
                Description = entity.Description,
                SubjectTemplate = entity.SubjectTemplate,
                SubjectVariables = entity.SubjectVariables,
                BodyTemplate = entity.BodyTemplate,
                BodyVariables = entity.BodyVariables,
                FromEmailOverride = entity.FromEmailOverride,
                Status = entity.Status,
                CreatedBy = entity.CreatedBy,
                CreatedDate = entity.CreatedDate,
                UpdatedBy = entity.UpdatedBy,
                UpdatedDate = entity.UpdatedDate
            };

            return Result<EmailTemplateResponseDto>.Success(response);
        }
    }
}
