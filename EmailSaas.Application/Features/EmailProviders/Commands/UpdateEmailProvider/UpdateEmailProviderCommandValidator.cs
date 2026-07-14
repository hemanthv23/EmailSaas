using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.Features.EmailProviders.Commands.UpdateEmailProvider
{
    public class UpdateEmailProviderCommandValidator : AbstractValidator<UpdateEmailProviderCommand>
    {
        public UpdateEmailProviderCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);

            RuleFor(x => x.ProviderName)
                .NotEmpty().MaximumLength(100);

            RuleFor(x => x.SenderName)
                .NotEmpty().MaximumLength(200);

            RuleFor(x => x.SenderEmail)
                .NotEmpty().EmailAddress().MaximumLength(200);

            RuleFor(x => x.ReplyToEmail)
                .EmailAddress().MaximumLength(200)
                .When(x => !string.IsNullOrEmpty(x.ReplyToEmail));

            RuleFor(x => x.UpdatedBy)
                .NotEmpty().MaximumLength(100);

            // SMTP-style provider rules
            When(x => !string.IsNullOrEmpty(x.SmtpHost), () =>
            {
                RuleFor(x => x.SmtpPort)
                    .NotNull().WithMessage("SmtpPort is required when SmtpHost is provided.");

                RuleFor(x => x.UserName)
                    .NotEmpty().WithMessage("UserName is required when SmtpHost is provided.");
            });

            // API-key based provider rules — Password not mandatory on update
            // (only required if user wants to change it)
        }
    }
}
