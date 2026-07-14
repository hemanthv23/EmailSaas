using EmailSaas.Application.Common.Interfaces;
using EmailSaas.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.Features.Webhooks.Commands.RegenerateWebhookSecret
{
    public class RegenerateWebhookSecretCommandHandler : IRequestHandler<RegenerateWebhookSecretCommand, Result<string>>
    {
        private readonly IApplicationDbContext _context;

        public RegenerateWebhookSecretCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<string>> Handle(RegenerateWebhookSecretCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.WebhookSubscriptions
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity == null)
                return Result<string>.Failure($"WebhookSubscription with Id '{request.Id}' not found.");

            var newSecret = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
                .Replace("+", "").Replace("/", "").Replace("=", "");

            entity.Secret = newSecret;
            entity.UpdatedBy = request.UpdatedBy;
            entity.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return Result<string>.Success(newSecret);
        }
    }
}
