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
                .FirstOrDefaultAsync(x => x.MessageId == request.MessageId, cancellationToken);

            if (emailLog == null)
                return Result<string>.Success(request.OriginalUrl);

            var now = DateTime.UtcNow;

            if (emailLog.ClickedAt == null)
                emailLog.ClickedAt = now;

            emailLog.LastClickedAt = now;
            emailLog.ClickCount += 1;
            emailLog.WebhookStatus = EmailEventType.Clicked.ToString();
            emailLog.UpdatedDate = now;

            _context.EmailLinkClicks.Add(new EmailLinkClick
            {
                EmailLogId = emailLog.Id,
                OriginalUrl = request.OriginalUrl,
                ClickedAt = now,
                IpAddress = request.IpAddress,
                UserAgent = request.UserAgent,
                CreatedBy = "System",
                CreatedDate = now
            });

            var eventData = new
            {
                request.OriginalUrl,
                request.IpAddress,
                request.UserAgent,
                ClickCount = emailLog.ClickCount
            };

            _context.EmailEvents.Add(new EmailEvent
            {
                EmailLogId = emailLog.Id,
                EventType = EmailEventType.Clicked.ToString(),
                EventData = JsonSerializer.Serialize(eventData),
                OccurredAt = now,
                CreatedBy = "System",
                CreatedDate = now
            });

            await _context.SaveChangesAsync(cancellationToken);

            await _webhookDispatcher.QueueWebhookAsync(emailLog.Id, EmailEventType.Clicked.ToString(), eventData, cancellationToken);

            return Result<string>.Success(request.OriginalUrl);
        }
    }
}