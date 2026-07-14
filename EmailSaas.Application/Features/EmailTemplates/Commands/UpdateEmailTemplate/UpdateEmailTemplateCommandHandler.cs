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

namespace EmailSaas.Application.Features.EmailTemplates.Commands.UpdateEmailTemplate
{
    public class UpdateEmailTemplateCommandHandler : IRequestHandler<UpdateEmailTemplateCommand, Result<EmailTemplateResponseDto>>
    {
        private readonly IApplicationDbContext _context;

        public UpdateEmailTemplateCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<EmailTemplateResponseDto>> Handle(
            UpdateEmailTemplateCommand request,
            CancellationToken cancellationToken)
        {
            var entity = await _context.EmailTemplateMasters
                .Include(x => x.Client)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity == null)
                throw new NotFoundException("EmailTemplateMaster", request.Id);

            entity.TemplateName = request.TemplateName;
            entity.Description = request.Description;
            entity.SubjectTemplate = request.SubjectTemplate;
            entity.SubjectVariables = request.SubjectVariables;
            entity.BodyTemplate = request.BodyTemplate;
            entity.BodyVariables = request.BodyVariables;
            entity.FromEmailOverride = request.FromEmailOverride;
            entity.UpdatedBy = request.UpdatedBy;
            entity.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            var response = new EmailTemplateResponseDto
            {
                Id = entity.Id,
                ClientId = entity.ClientId,
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
