using EmailSaas.Application.Common.Interfaces;
using EmailSaas.Application.Common.Models;
using EmailSaas.Application.DTOs.Webhook;
using EmailSaas.Domain.Entities;
using EmailSaas.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.Features.Webhooks.Commands.CreateWebhookSubscription
{
    public class CreateWebhookSubscriptionCommandHandler
         : IRequestHandler<CreateWebhookSubscriptionCommand, Result<WebhookSubscriptionResponseDto>>
    {
        private readonly IApplicationDbContext _context;

        public CreateWebhookSubscriptionCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<WebhookSubscriptionResponseDto>> Handle(
            CreateWebhookSubscriptionCommand request, CancellationToken cancellationToken)
        {
            var client = await _context.ClientMasters
                .FirstOrDefaultAsync(x => x.Id == request.ClientId, cancellationToken);

            if (client == null)
                return Result<WebhookSubscriptionResponseDto>.Failure(
                    $"Client with Id '{request.ClientId}' not found.");

            // Prevent duplicate: same client + same callback URL already registered
            var duplicate = await _context.WebhookSubscriptions
                .AnyAsync(x => x.ClientId == request.ClientId
                             && x.CallbackUrl == request.CallbackUrl
                             && x.Status == (byte)CommonStatus.Active, cancellationToken);

            if (duplicate)
                return Result<WebhookSubscriptionResponseDto>.Failure(
                    "An active webhook subscription with this CallbackUrl already exists for this client.");

            var secret = GenerateSecret();

            var entity = new WebhookSubscription
            {
                ClientId = request.ClientId,
                CallbackUrl = request.CallbackUrl,
                Secret = secret,
                EventTypes = string.Join(",", request.EventTypes),
                Status = (byte)CommonStatus.Active,
                CreatedBy = request.CreatedBy,
                CreatedDate = DateTime.UtcNow
            };

            _context.WebhookSubscriptions.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            var response = new WebhookSubscriptionResponseDto
            {
                Id = entity.Id,
                ClientId = entity.ClientId,
                ClientName = client.ClientName,
                CallbackUrl = entity.CallbackUrl,
                Secret = entity.Secret, // returned only here, on creation — client must store it now
                EventTypes = entity.EventTypes.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList(),
                Status = entity.Status,
                CreatedBy = entity.CreatedBy,
                CreatedDate = entity.CreatedDate,
                UpdatedBy = entity.UpdatedBy,
                UpdatedDate = entity.UpdatedDate
            };

            return Result<WebhookSubscriptionResponseDto>.Success(response);
        }

        private static string GenerateSecret()
        {
            var bytes = RandomNumberGenerator.GetBytes(32);
            return Convert.ToBase64String(bytes).Replace("+", "").Replace("/", "").Replace("=", "");
        }
    }
}
