/*
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
            var client = await _context.MasterClients
                .FirstOrDefaultAsync(x => x.Id == request.ClientID, cancellationToken);

            if (client == null)
                return Result<WebhookSubscriptionResponseDto>.Failure(
                    $"Client with Id '{request.ClientID}' not found.");

            // Prevent duplicate: same client + same callback URL already registered
            var duplicate = await _context.WebhookSubscriptions
                .AnyAsync(x => x.ClientID == request.ClientID
                             && x.CallbackUrl == request.CallbackUrl
                             && x.Status == (byte)CommonStatus.Active, cancellationToken);

            if (duplicate)
                return Result<WebhookSubscriptionResponseDto>.Failure(
                    "An active webhook subscription with this CallbackUrl already exists for this client.");

            var secret = GenerateSecret();

            var entity = new WebhookSubscription
            {
                ClientID = request.ClientID,
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
                ClientID = entity.ClientID,
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

*/