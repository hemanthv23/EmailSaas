using EmailSaas.Application.Common.Interfaces;
using EmailSaas.Application.Common.Models;
using EmailSaas.Application.DTOs.EmailTemplate;
using EmailSaas.Domain.Entities;
using EmailSaas.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.Features.EmailTemplates.Commands.CreateEmailTemplate
{
    public class CreateEmailTemplateCommandHandler : IRequestHandler<CreateEmailTemplateCommand, Result<EmailTemplateResponseDto>>
    {
        private readonly IApplicationDbContext _context;

        public CreateEmailTemplateCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<EmailTemplateResponseDto>> Handle(CreateEmailTemplateCommand request, CancellationToken cancellationToken)
        {
            // Check client exists
            var client = await _context.MasterClients
                .FirstOrDefaultAsync(x => x.Id == request.ClientID, cancellationToken);

            if (client == null)
                return Result<EmailTemplateResponseDto>.Failure($"Client with Id '{request.ClientID}' not found.");

            // Check duplicate TemplateCode for same client
            var exists = await _context.MasterEmailTemplates
                .AnyAsync(x => x.ClientID == request.ClientID
                            && x.TemplateCode == request.TemplateCode, cancellationToken);

            if (exists)
                return Result<EmailTemplateResponseDto>.Failure($"TemplateCode '{request.TemplateCode}' already exists for this client.");

            var entity = new MasterEmailTemplate
            {
                ApplicationId = request.ApplicationId,
                ClientID = request.ClientID,
                TemplateCode = request.TemplateCode,
                TemplateName = request.TemplateName,
                Description = request.Description,
                SubjectTemplate = request.SubjectTemplate,
                SubjectVariables = request.SubjectVariables,
                BodyTemplate = request.BodyTemplate,
                BodyVariables = request.BodyVariables,
                FromEmailOverride = request.FromEmailOverride,
                Status = (byte)CommonStatus.Active,
                CreatedBy = request.CreatedBy,
                CreatedDate = DateTime.UtcNow
            };

            _context.MasterEmailTemplates.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            var response = new EmailTemplateResponseDto
            {
                Id = entity.Id,
                ClientID = entity.ClientID,
                ClientCode = client.ClientCode,
                ClientName = client.ClientName,
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
