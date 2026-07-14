using FluentValidation;

namespace EmailSaas.Application.Features.Applications.Commands.CreateApplication;

public class CreateApplicationCommandValidator : AbstractValidator<CreateApplicationCommand>
{
    public CreateApplicationCommandValidator()
    {
        RuleFor(x => x.ApplicationCode).NotEmpty().MaximumLength(50);
        RuleFor(x => x.ApplicationName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.CreatedBy).NotEmpty().MaximumLength(100);
    }
}