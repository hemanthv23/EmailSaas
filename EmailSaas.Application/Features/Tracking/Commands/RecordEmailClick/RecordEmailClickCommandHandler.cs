using EmailSaas.Application.Common.Interfaces;
using EmailSaas.Application.Common.Models;
using EmailSaas.Domain.Entities;
using EmailSaas.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace EmailSaas.Application.Features.Tracking.Commands.RecordEmailClick
{
    public class RecordEmailClickCommandHandler : IRequestHandler<RecordEmailClickCommand, Result<string>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IWebhookDispatcher _webhookDispatcher;

        public RecordEmailClickCommandHandler(IApplicationDbContext context, IWebhookDispatcher webhookDispatcher)
        {
            _context = context;
            _webhookDispatcher = webhookDispatcher;
        }

        public async Task<Result<string>> Handle(RecordEmailClickCommand request, CancellationToken cancellationToken)
        {
            var emailLog = await _context.EmailLogs
                .FirstOrDefaultAsync(x => x.MessageID == request.MessageID, cancellationToken);

            if (emailLog == null)
                return Result<string>.Success(request.OriginalUrl);

            var now = DateTime.UtcNow;

            // --- NEW: Implicit Open Tracking ---
            // If they clicked a link, they definitely opened the email.
            // If the image was blocked, we can use this click to record the Open event too!
            if (emailLog.Status != (byte)EmailSendStatus.Delivered)
            {
                emailLog.Status = (byte)EmailSendStatus.Delivered;

                // Fire an "Opened" event to the Events table
                _context.EmailEventLogs.Add(new EmailEventLog
                {
                    LogID = emailLog.Id,
                    MessageID = emailLog.MessageID ?? request.MessageID,
                    EventType = EmailEventLogType.Opened.ToString(),
                    LogData = JsonSerializer.Serialize(new { request.IpAddress, request.UserAgent, Note = "Inferred from Click" }),
                    EventLogDate = now,
                    Status = 1,
                    CreatedBy = "System",
                    CreatedDate = now
                });
            }

            emailLog.UpdatedDate = now;

            var eventData = new
            {
                request.OriginalUrl,
                request.IpAddress,
                request.UserAgent
            };

            _context.EmailEventLogs.Add(new EmailEventLog
            {
                LogID = emailLog.Id,
                MessageID = emailLog.MessageID ?? request.MessageID,
                EventType = EmailEventLogType.Clicked.ToString(),
                LogData = JsonSerializer.Serialize(eventData),
                EventLogDate = now,
                Status = 1,
                CreatedBy = "System",
                CreatedDate = now
            });

            await _context.SaveChangesAsync(cancellationToken);

            await _webhookDispatcher.QueueWebhookAsync(emailLog.Id, EmailEventLogType.Clicked.ToString(), eventData, cancellationToken);

            return Result<string>.Success(request.OriginalUrl);
        }
    }
}