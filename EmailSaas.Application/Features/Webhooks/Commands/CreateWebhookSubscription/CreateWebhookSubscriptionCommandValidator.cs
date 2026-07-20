/*
using EmailSaas.Domain.Enums;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.Features.Webhooks.Commands.CreateWebhookSubscription
{
    public class CreateWebhookSubscriptionCommandValidator : AbstractValidator<CreateWebhookSubscriptionCommand>
    {
        private static readonly HashSet<string> ValidEventTypes = Enum.GetNames(typeof(WebhookEventType))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        public CreateWebhookSubscriptionCommandValidator()
        {
            RuleFor(x => x.ClientID).GreaterThan(0).WithMessage("Valid ClientID is required.");

            RuleFor(x => x.CallbackUrl)
                .NotEmpty().WithMessage("CallbackUrl is required.")
                .MaximumLength(500)
                .Must(BeAValidHttpsUrl).WithMessage("CallbackUrl must be a valid HTTPS URL.");

            RuleFor(x => x.EventTypes)
                .NotEmpty().WithMessage("At least one EventType is required.")
                .Must(events => events.All(e => ValidEventTypes.Contains(e)))
                .WithMessage($"EventTypes must be one of: {string.Join(", ", ValidEventTypes)}");

            RuleFor(x => x.CreatedBy).NotEmpty().MaximumLength(100);
        }

        private static bool BeAValidHttpsUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var result)
                   && result.Scheme == Uri.UriSchemeHttps; // enforce HTTPS — no plain HTTP callback URLs
        }
    }
}

*/