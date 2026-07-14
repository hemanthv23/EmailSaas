using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.Features.Clients.Commands.CreateClient
{
    public class CreateClientCommandValidator : AbstractValidator<CreateClientCommand>
    {
        public CreateClientCommandValidator()
        {
            RuleFor(x => x.ApplicationId).GreaterThan(0).WithMessage("Valid ApplicationId is required.");
            RuleFor(x => x.ClientCode).NotEmpty().MaximumLength(50);
            RuleFor(x => x.ClientName).NotEmpty().MaximumLength(200);
            RuleFor(x => x.LogoUrl).MaximumLength(500).When(x => x.LogoUrl != null);
            RuleFor(x => x.PrimaryColor).MaximumLength(50).When(x => x.PrimaryColor != null);
            RuleFor(x => x.CreatedBy).NotEmpty().MaximumLength(100);
        }
    }
}
