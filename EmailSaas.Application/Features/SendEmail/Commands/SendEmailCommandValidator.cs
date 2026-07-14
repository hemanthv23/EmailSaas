using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.Features.SendEmail.Commands
{
    public class SendEmailCommandValidator : AbstractValidator<SendEmailCommand>
    {
        public SendEmailCommandValidator()
        {
            RuleFor(x => x.ApplicationId)
                .GreaterThan(0).WithMessage("Valid ApplicationId is required.");

            RuleFor(x => x.ClientId)
                .GreaterThan(0).WithMessage("Valid ClientId is required.");

            RuleFor(x => x.TemplateCode)
                .NotEmpty().WithMessage("TemplateCode is required.");

            RuleFor(x => x.ToEmail)
                .NotEmpty().WithMessage("ToEmail is required.")
                .EmailAddress().WithMessage("ToEmail must be a valid email address.");

            RuleFor(x => x.CcEmail)
                .EmailAddress().WithMessage("CcEmail must be a valid email address.")
                .When(x => !string.IsNullOrEmpty(x.CcEmail));

            RuleFor(x => x.BccEmail)
                .EmailAddress().WithMessage("BccEmail must be a valid email address.")
                .When(x => !string.IsNullOrEmpty(x.BccEmail));

            RuleFor(x => x.CreatedBy)
                .NotEmpty().WithMessage("CreatedBy is required.");
        }
    }
}
