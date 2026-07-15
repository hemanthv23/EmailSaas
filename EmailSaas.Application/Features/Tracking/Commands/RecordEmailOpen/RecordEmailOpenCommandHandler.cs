using EmailSaas.Application.Common.Interfaces;
using EmailSaas.Application.Common.Models;
using EmailSaas.Domain.Entities;
using EmailSaas.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace EmailSaas.Application.Features.Tracking.Commands.RecordEmailOpen
{
    public class RecordEmailOpenCommandHandler : IRequestHandler<RecordEmailOpenCommand, Result<bool>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IWebhookDispatcher _webhookDispatcher;

        public RecordEmailOpenCommandHandler(IApplicationDbContext context, IWebhookDispatcher webhookDispatcher)
        {
            _context = context;
            _webhookDispatcher = webhookDispatcher;
        }

        public async Task<Result<bool>> Handle(RecordEmailOpenCommand request, CancellationToken cancellationToken)
        {
            var emailLog = await _context.EmailLogs
                .FirstOrDefaultAsync(x => x.MessageId == request.MessageId, cancellationToken);

            if (emailLog == null)
                return Result<bool>.Failure($"EmailLog with MessageId '{request.MessageId}' not found.");

            var now = DateTime.UtcNow;

            if (emailLog.OpenedAt == null)
            {
                emailLog.OpenedAt = now;

                // First open = proof email reached the inbox = mark as Delivered
                emailLog.DeliveredAt = now;
                emailLog.Status = (byte)EmailSendStatus.Delivered;
                emailLog.WebhookStatus = EmailEventType.Delivered.ToString();
            }

            emailLog.LastOpenedAt = now;
            emailLog.OpenCount += 1;
            emailLog.UpdatedDate = now;

            var eventData = new
            {
                request.IpAddress,
                request.UserAgent,
                OpenCount = emailLog.OpenCount
            };

            _context.EmailEvents.Add(new EmailEvent
            {
                EmailLogId = emailLog.Id,
                EventType = EmailEventType.Opened.ToString(),
                EventData = JsonSerializer.Serialize(eventData),
                OccurredAt = now,
                CreatedBy = "System",
                CreatedDate = now
            });

            await _context.SaveChangesAsync(cancellationToken);

            await _webhookDispatcher.QueueWebhookAsync(emailLog.Id, EmailEventType.Opened.ToString(), eventData, cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}