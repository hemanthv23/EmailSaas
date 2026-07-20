/*
using EmailSaas.Application.Common.Interfaces;
using EmailSaas.Application.Common.Models;
using EmailSaas.Application.DTOs.Webhook;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.Features.Webhooks.Queries.GetWebhookSubscriptionById
{
    public class GetWebhookSubscriptionByIdQueryHandler
        : IRequestHandler<GetWebhookSubscriptionByIdQuery, Result<WebhookSubscriptionResponseDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetWebhookSubscriptionByIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<WebhookSubscriptionResponseDto>> Handle(
            GetWebhookSubscriptionByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _context.WebhookSubscriptions
                .Include(x => x.Client)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity == null)
                return Result<WebhookSubscriptionResponseDto>.Failure(
                    $"WebhookSubscription with Id '{request.Id}' not found.");

            var response = new WebhookSubscriptionResponseDto
            {
                Id = entity.Id,
                ClientID = entity.ClientID,
                ClientName = entity.Client.ClientName,
                CallbackUrl = entity.CallbackUrl,
                Secret = "••••••••",
                EventTypes = entity.EventTypes.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList(),
                Status = entity.Status,
                CreatedBy = entity.CreatedBy,
                CreatedDate = entity.CreatedDate,
                UpdatedBy = entity.UpdatedBy,
                UpdatedDate = entity.UpdatedDate
            };

            return Result<WebhookSubscriptionResponseDto>.Success(response);
        }
    }
}

*/