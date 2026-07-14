using MediatR;
using Microsoft.EntityFrameworkCore;
using EmailSaas.Application.Common.Interfaces;
using EmailSaas.Application.Common.Models;
using EmailSaas.Application.DTOs.EmailLog;
using EmailSaas.Application.Common.Exceptions;
using EmailSaas.Domain.Enums;

namespace EmailSaas.Application.Features.EmailLogs.Queries.GetEmailLogById;

public class GetEmailLogByIdQueryHandler : IRequestHandler<GetEmailLogByIdQuery, Result<EmailLogResponseDto>>
{
    private readonly IApplicationDbContext _context;

    public GetEmailLogByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<EmailLogResponseDto>> Handle(GetEmailLogByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.EmailLogs
            .AsNoTracking()
            .Include(x => x.Application)
            .Include(x => x.Client)
            .Include(x => x.Template)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
            throw new NotFoundException("EmailLog", request.Id);

        var response = new EmailLogResponseDto
        {
            Id = entity.Id,
            ApplicationId = entity.ApplicationId,
            ApplicationCode = entity.Application.ApplicationCode,
            ApplicationName = entity.Application.ApplicationName,
            ClientId = entity.ClientId,
            ClientCode = entity.Client.ClientCode,
            ClientName = entity.Client.ClientName,
            TemplateId = entity.TemplateId,
            TemplateCode = entity.Template.TemplateCode,
            TemplateName = entity.Template.TemplateName,
            ToEmail = entity.ToEmail,
            CcEmail = entity.CcEmail,
            BccEmail = entity.BccEmail,
            //Subject = entity.Subject,
            //ParameterValues = entity.ParameterValues,
            //RenderedBody = entity.RenderedBody,
            Status = entity.Status == (byte)EmailSendStatus.Sent ? "Sent"
                   : entity.Status == (byte)EmailSendStatus.Failed ? "Failed"
                   : "Pending",
            ErrorMessage = entity.ErrorMessage,
            SentDate = entity.SentDate,
            CreatedBy = entity.CreatedBy,
            CreatedDate = entity.CreatedDate,
            UpdatedBy = entity.UpdatedBy,
            UpdatedDate = entity.UpdatedDate,

            // ─── Webhook tracking ─────────────────────────────────────
            MessageId = entity.MessageId,
            WebhookStatus = entity.WebhookStatus,

            // ─── Delivery tracking ────────────────────────────────────
            DeliveredAt = entity.DeliveredAt,

            // ─── Open tracking ────────────────────────────────────────
            OpenedAt = entity.OpenedAt,
            LastOpenedAt = entity.LastOpenedAt,
            OpenCount = entity.OpenCount,

            // ─── Attachment tracking ──────────────────────────────────
            HasAttachment = entity.HasAttachment,
            AttachmentNames = entity.AttachmentNames,
            AttachmentOpenedAt = entity.AttachmentOpenedAt,
            AttachmentOpenCount = entity.AttachmentOpenCount,

            // ─── Bounce tracking ──────────────────────────────────────
            BouncedAt = entity.BouncedAt,
            BounceReason = entity.BounceReason
        };

        return Result<EmailLogResponseDto>.Success(response);
    }
}