/*
using EmailSaas.Application.Common.Interfaces;
using EmailSaas.Application.Common.Models;
using EmailSaas.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.Features.Webhooks.Commands.DeleteWebhookSubscription
{
    public class DeleteWebhookSubscriptionCommandHandler : IRequestHandler<DeleteWebhookSubscriptionCommand, Result<bool>>
    {
        private readonly IApplicationDbContext _context;

        public DeleteWebhookSubscriptionCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<bool>> Handle(DeleteWebhookSubscriptionCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.WebhookSubscriptions
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity == null)
                return Result<bool>.Failure($"WebhookSubscription with Id '{request.Id}' not found.");

            // Soft delete — keeps history for WebhookDeliveryLog FK integrity
            entity.Status = (byte)CommonStatus.Inactive;
            entity.UpdatedBy = request.UpdatedBy;
            entity.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}

*/