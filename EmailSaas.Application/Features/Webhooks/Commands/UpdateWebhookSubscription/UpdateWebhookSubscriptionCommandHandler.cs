using EmailSaas.Application.Common.Interfaces;
using EmailSaas.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.Features.Webhooks.Commands.UpdateWebhookSubscription
{
    public class UpdateWebhookSubscriptionCommandHandler : IRequestHandler<UpdateWebhookSubscriptionCommand, Result<bool>>
    {
        private readonly IApplicationDbContext _context;

        public UpdateWebhookSubscriptionCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<bool>> Handle(UpdateWebhookSubscriptionCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.WebhookSubscriptions
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity == null)
                return Result<bool>.Failure($"WebhookSubscription with Id '{request.Id}' not found.");

            entity.CallbackUrl = request.CallbackUrl;
            entity.EventTypes = string.Join(",", request.EventTypes);
            entity.Status = request.Status;
            entity.UpdatedBy = request.UpdatedBy;
            entity.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
