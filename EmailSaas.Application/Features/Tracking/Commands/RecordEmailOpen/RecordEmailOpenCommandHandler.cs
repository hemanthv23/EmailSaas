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
                .FirstOrDefaultAsync(x => x.MessageID == request.MessageID, cancellationToken);

            if (emailLog == null)
                return Result<bool>.Failure($"EmailLog with MessageID '{request.MessageID}' not found.");

            var now = DateTime.UtcNow;

            // Implicit Delivery Tracking:
            // If the email is opened, it must have been delivered.
            if (emailLog.Status != (byte)EmailSendStatus.Delivered)
            {
                emailLog.Status = (byte)EmailSendStatus.Delivered;
            }

            emailLog.UpdatedDate = now;

            var eventData = new
            {
                request.IpAddress,
                request.UserAgent
            };

            _context.EmailEventLogs.Add(new EmailEventLog
            {
                LogID = emailLog.Id,
                MessageID = emailLog.MessageID ?? request.MessageID,
                EventType = EmailEventLogType.Opened.ToString(),
                LogData = JsonSerializer.Serialize(eventData),
                EventLogDate = now,
                Status = 1,
                CreatedBy = "System",
                CreatedDate = now
            });

            await _context.SaveChangesAsync(cancellationToken);

            await _webhookDispatcher.QueueWebhookAsync(emailLog.Id, EmailEventLogType.Opened.ToString(), eventData, cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}