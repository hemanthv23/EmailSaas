using MediatR;
using Microsoft.EntityFrameworkCore;
using EmailSaas.Application.Common.Interfaces;
using EmailSaas.Application.Common.Models;
using EmailSaas.Application.DTOs.EmailTemplate;

namespace EmailSaas.Application.Features.EmailTemplates.Queries.GetAllEmailTemplates;

public class GetAllEmailTemplatesQueryHandler : IRequestHandler<GetAllEmailTemplatesQuery, Result<List<EmailTemplateResponseDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetAllEmailTemplatesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<EmailTemplateResponseDto>>> Handle(GetAllEmailTemplatesQuery request, CancellationToken cancellationToken)
    {
        var templates = await _context.EmailTemplateMasters
            .AsNoTracking()
            .Include(x => x.Client)
            .ThenInclude(x => x.Application)
            .Where(x => x.Client.ApplicationId == request.ApplicationId)
            .OrderByDescending(x => x.CreatedDate)
            .Select(x => new EmailTemplateResponseDto
            {
                Id = x.Id,
                ClientId = x.ClientId,
                ClientCode = x.Client.ClientCode,
                ClientName = x.Client.ClientName,
                TemplateCode = x.TemplateCode,
                TemplateName = x.TemplateName,
                Description = x.Description,
                SubjectTemplate = x.SubjectTemplate,
                SubjectVariables = x.SubjectVariables,
                BodyTemplate = x.BodyTemplate,
                BodyVariables = x.BodyVariables,
                FromEmailOverride = x.FromEmailOverride,
                Status = x.Status,
                CreatedBy = x.CreatedBy,
                CreatedDate = x.CreatedDate,
                UpdatedBy = x.UpdatedBy,
                UpdatedDate = x.UpdatedDate
            })
            .ToListAsync(cancellationToken);

        return Result<List<EmailTemplateResponseDto>>.Success(templates);
    }
}