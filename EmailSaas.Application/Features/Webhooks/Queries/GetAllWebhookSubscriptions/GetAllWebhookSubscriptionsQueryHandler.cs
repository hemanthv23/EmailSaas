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

namespace EmailSaas.Application.Features.Webhooks.Queries.GetAllWebhookSubscriptions
{
    public class GetAllWebhookSubscriptionsQueryHandler
         : IRequestHandler<GetAllWebhookSubscriptionsQuery, Result<PaginatedList<WebhookSubscriptionResponseDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllWebhookSubscriptionsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PaginatedList<WebhookSubscriptionResponseDto>>> Handle(
            GetAllWebhookSubscriptionsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.WebhookSubscriptions
                .Include(x => x.Client)
                .Where(x => x.ClientId == request.ClientId)
                .OrderByDescending(x => x.CreatedDate)
                .Select(x => new WebhookSubscriptionResponseDto
                {
                    Id = x.Id,
                    ClientId = x.ClientId,
                    ClientName = x.Client.ClientName,
                    CallbackUrl = x.CallbackUrl,
                    Secret = "••••••••", // never expose secret again after creation
                    EventTypes = x.EventTypes.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList(),
                    Status = x.Status,
                    CreatedBy = x.CreatedBy,
                    CreatedDate = x.CreatedDate,
                    UpdatedBy = x.UpdatedBy,
                    UpdatedDate = x.UpdatedDate
                });

            var result = await PaginatedList<WebhookSubscriptionResponseDto>.CreateAsync(
                query, request.PageNumber, request.PageSize);

            return Result<PaginatedList<WebhookSubscriptionResponseDto>>.Success(result);
        }
    }
}
