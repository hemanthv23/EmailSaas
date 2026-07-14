using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.Features.Applications.Commands.RegenerateApiKey
{
    public class RegenerateApiKeyCommandValidator : AbstractValidator<RegenerateApiKeyCommand>
    {
        public RegenerateApiKeyCommandValidator()
        {
            RuleFor(x => x.ApplicationCode)
                .NotEmpty().WithMessage("ApplicationCode is required.")
                .MaximumLength(50);

            RuleFor(x => x.UpdatedBy)
                .NotEmpty().WithMessage("UpdatedBy is required.")
                .MaximumLength(100);
        }
    }
}
