using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.Features.EmailTemplates.Commands.UpdateEmailTemplate
{
    public class UpdateEmailTemplateCommandValidator : AbstractValidator<UpdateEmailTemplateCommand>
    {
        public UpdateEmailTemplateCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);

            RuleFor(x => x.TemplateName)
                .NotEmpty().MaximumLength(200);

            RuleFor(x => x.SubjectTemplate)
                .NotEmpty().MaximumLength(500);

            RuleFor(x => x.BodyTemplate)
                .NotEmpty();

            RuleFor(x => x.FromEmailOverride)
                .EmailAddress().MaximumLength(200)
                .When(x => !string.IsNullOrEmpty(x.FromEmailOverride));

            RuleFor(x => x.UpdatedBy)
                .NotEmpty().MaximumLength(100);
        }
    }
}
