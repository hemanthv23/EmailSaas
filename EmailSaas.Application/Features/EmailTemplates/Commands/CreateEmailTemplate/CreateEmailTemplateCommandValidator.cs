using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.Features.EmailTemplates.Commands.CreateEmailTemplate
{
    public class CreateEmailTemplateCommandValidator : AbstractValidator<CreateEmailTemplateCommand>
    {
        public CreateEmailTemplateCommandValidator()
        {
            RuleFor(x => x.ClientID)
                .GreaterThan(0).WithMessage("Valid ClientID is required.");

            RuleFor(x => x.TemplateCode)
                .NotEmpty().WithMessage("TemplateCode is required.")
                .MaximumLength(100);

            RuleFor(x => x.TemplateName)
                .NotEmpty().WithMessage("TemplateName is required.")
                .MaximumLength(200);

            RuleFor(x => x.Description)
                .MaximumLength(500).When(x => x.Description != null);

            RuleFor(x => x.SubjectTemplate)
                .NotEmpty().WithMessage("SubjectTemplate is required.")
                .MaximumLength(500);

            RuleFor(x => x.BodyTemplate)
                .NotEmpty().WithMessage("BodyTemplate is required.");

            RuleFor(x => x.FromEmailOverride)
                .EmailAddress().WithMessage("FromEmailOverride must be a valid email.")
                .MaximumLength(200)
                .When(x => x.FromEmailOverride != null);

            RuleFor(x => x.CreatedBy)
                .NotEmpty().WithMessage("CreatedBy is required.")
                .MaximumLength(100);
        }
    }
}
