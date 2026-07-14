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

namespace EmailSaas.Application.Features.Tracking.Commands.RecordEmailDelivered
{
    public class RecordEmailDeliveredCommandHandler : IRequestHandler<RecordEmailDeliveredCommand, Result<bool>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IWebhookDispatcher _webhookDispatcher;

        public RecordEmailDeliveredCommandHandler(IApplicationDbContext context, IWebhookDispatcher webhookDispatcher)
        {
            _context = context;
            _webhookDispatcher = webhookDispatcher;
        }

        public async Task<Result<bool>> Handle(RecordEmailDeliveredCommand request, CancellationToken cancellationToken)
        {
            var emailLog = await _context.EmailLogs
                .FirstOrDefaultAsync(x => x.MessageId == request.MessageId, cancellationToken);

            if (emailLog == null)
                return Result<bool>.Failure($"EmailLog with MessageId '{request.MessageId}' not found.");

            var now = DateTime.UtcNow;

            emailLog.Status = (byte)EmailSendStatus.Delivered;
            emailLog.DeliveredAt = now;
            emailLog.WebhookStatus = EmailEventType.Delivered.ToString();
            emailLog.UpdatedDate = now;

            var eventData = new { request.ProviderResponse };

            _context.EmailEvents.Add(new EmailEvent
            {
                EmailLogId = emailLog.Id,
                EventType = EmailEventType.Delivered.ToString(),
                EventData = JsonSerializer.Serialize(eventData),
                OccurredAt = now,
                CreatedBy = "System",
                CreatedDate = now
            });

            await _context.SaveChangesAsync(cancellationToken);

            await _webhookDispatcher.QueueWebhookAsync(emailLog.Id, EmailEventType.Delivered.ToString(), eventData, cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
