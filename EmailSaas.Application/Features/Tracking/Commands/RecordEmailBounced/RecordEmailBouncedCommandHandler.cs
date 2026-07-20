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
                .FirstOrDefaultAsync(x => x.MessageID == request.MessageID, cancellationToken);

            if (emailLog == null)
                return Result<bool>.Failure($"EmailLog with MessageID '{request.MessageID}' not found.");

            var now = DateTime.UtcNow;

            emailLog.Status = (byte)EmailSendStatus.Bounced;
            emailLog.UpdatedDate = now;
            emailLog.UpdatedDate = now;

            var eventData = new { request.BounceReason, request.IsHardBounce };

            _context.EmailEventLogs.Add(new EmailEventLog
            {
                LogID = emailLog.Id,
                MessageID = emailLog.MessageID ?? request.MessageID,
                EventType = EmailEventLogType.Bounced.ToString(),
                LogData = JsonSerializer.Serialize(eventData),
                EventLogDate = now,
                Status = 1,
                CreatedBy = "System",
                CreatedDate = now
            });

            await _context.SaveChangesAsync(cancellationToken);

            await _webhookDispatcher.QueueWebhookAsync(emailLog.Id, EmailEventLogType.Bounced.ToString(), eventData, cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}