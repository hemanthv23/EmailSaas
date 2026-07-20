/*
using EmailSaas.Domain.Enums;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.Features.Webhooks.Commands.UpdateWebhookSubscription
{
    public class UpdateWebhookSubscriptionCommandValidator : AbstractValidator<UpdateWebhookSubscriptionCommand>
    {
        private static readonly HashSet<string> ValidEventTypes = Enum.GetNames(typeof(WebhookEventType))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        public UpdateWebhookSubscriptionCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);

            RuleFor(x => x.CallbackUrl)
                .NotEmpty().MaximumLength(500)
                .Must(BeAValidHttpsUrl).WithMessage("CallbackUrl must be a valid HTTPS URL.");

            RuleFor(x => x.EventTypes)
                .NotEmpty()
                .Must(events => events.All(e => ValidEventTypes.Contains(e)))
                .WithMessage($"EventTypes must be one of: {string.Join(", ", ValidEventTypes)}");

            RuleFor(x => x.UpdatedBy).NotEmpty().MaximumLength(100);
        }

        private static bool BeAValidHttpsUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var result)
                   && result.Scheme == Uri.UriSchemeHttps;
        }
    }
}

*/