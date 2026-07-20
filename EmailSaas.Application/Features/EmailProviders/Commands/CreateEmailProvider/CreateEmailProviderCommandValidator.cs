using FluentValidation;

namespace EmailSaas.Application.Features.EmailProviders.Commands.CreateEmailProvider;

public class CreateEmailProviderCommandValidator : AbstractValidator<CreateEmailProviderCommand>
{
    public CreateEmailProviderCommandValidator()
    {
        RuleFor(x => x.ClientID)
            .GreaterThan(0)
            .WithMessage("Valid ClientID is required.");

        RuleFor(x => x.ProviderName)
            .NotEmpty().WithMessage("ProviderName is required.")
            .MaximumLength(100);
        // No hardcoded list — any provider name accepted

        RuleFor(x => x.SenderName)
            .NotEmpty().WithMessage("SenderName is required.")
            .MaximumLength(200);

        RuleFor(x => x.SenderEmail)
            .NotEmpty().WithMessage("SenderEmail is required.")
            .EmailAddress().WithMessage("SenderEmail must be a valid email.")
            .MaximumLength(200);

        RuleFor(x => x.ReplyToEmail)
            .EmailAddress().WithMessage("ReplyToEmail must be a valid email.")
            .MaximumLength(200)
            .When(x => !string.IsNullOrEmpty(x.ReplyToEmail));

        RuleFor(x => x.CreatedBy)
            .NotEmpty().WithMessage("CreatedBy is required.")
            .MaximumLength(100);

        // ✅ Microsoft Graph specific rules
        // TenantId (UserName), AzureClientID (ApiKey), ClientSecret (Password) required
        When(x => x.ProviderName == "MicrosoftGraph", () =>
        {
            RuleFor(x => x.UserName)
                .NotEmpty()
                .WithMessage("TenantId is required for MicrosoftGraph provider (stored in UserName field).");

            RuleFor(x => x.ApiKey)
                .NotEmpty()
                .WithMessage("AzureClientID is required for MicrosoftGraph provider (stored in ApiKey field).");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("ClientSecret is required for MicrosoftGraph provider (stored in Password field).");
        });

        // ─── SMTP-style provider rules (SMTPHost given) ───────
        When(x => x.ProviderName != "MicrosoftGraph"
                && !string.IsNullOrEmpty(x.SMTPHost), () =>
                {
                    RuleFor(x => x.SMTPPort)
                        .NotNull()
                        .WithMessage("SMTPPort is required when SMTPHost is provided.");

                    RuleFor(x => x.UserName)
                        .NotEmpty()
                        .WithMessage("UserName is required when SMTPHost is provided.")
                        .MaximumLength(200);

                    RuleFor(x => x.Password)
                        .NotEmpty()
                        .WithMessage("Password is required when SMTPHost is provided.");
                });

        // ─── API-key based provider rules ──
        When(x => x.ProviderName != "MicrosoftGraph"
                && string.IsNullOrEmpty(x.SMTPHost), () =>
                {
                    RuleFor(x => x.ApiKey)
                        .NotEmpty()
                        .WithMessage("ApiKey is required when SMTPHost is not provided (API-based provider).");
                });
    }
}
