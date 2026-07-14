using EmailSaas.Application.Common.Interfaces;
using EmailSaas.Application.Common.Models;
using EmailSaas.Domain.Entities;
using EmailSaas.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EmailSaas.Application.Features.Tracking.Commands.RecordEmailBounced
{
    public class RecordEmailBouncedCommandHandler : IRequestHandler<RecordEmailBouncedCommand, Result<bool>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IWebhookDispatcher _webhookDispatcher;

        public RecordEmailBouncedCommandHandler(IApplicationDbContext context, IWebhookDispatcher webhookDispatcher)
        {
            _context = context;
            _webhookDispatcher = webhookDispatcher;
        }

        public async Task<Result<bool>> Handle(RecordEmailBouncedCommand request, CancellationToken cancellationToken)
        {
            var emailLog = await _context.EmailLogs
                .FirstOrDefaultAsync(x => x.MessageId == request.MessageId, cancellationToken);

            if (emailLog == null)
                return Result<bool>.Failure($"EmailLog with MessageId '{request.MessageId}' not found.");

            var now = DateTime.UtcNow;

            emailLog.Status = (byte)EmailSendStatus.Bounced;
            emailLog.BouncedAt = now;
            emailLog.BounceReason = request.BounceReason;
            emailLog.WebhookStatus = EmailEventType.Bounced.ToString();
            emailLog.UpdatedDate = now;

            var eventData = new { request.BounceReason, request.IsHardBounce };

            _context.EmailEvents.Add(new EmailEvent
            {
                EmailLogId = emailLog.Id,
                EventType = EmailEventType.Bounced.ToString(),
                EventData = JsonSerializer.Serialize(eventData),
                OccurredAt = now,
                CreatedBy = "System",
                CreatedDate = now
            });

            await _context.SaveChangesAsync(cancellationToken);

            await _webhookDispatcher.QueueWebhookAsync(emailLog.Id, EmailEventType.Bounced.ToString(), eventData, cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
