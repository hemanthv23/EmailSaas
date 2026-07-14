using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.Features.Tracking.Commands.RecordEmailOpen
{
    public class RecordEmailOpenCommandValidator : AbstractValidator<RecordEmailOpenCommand>
    {
        public RecordEmailOpenCommandValidator()
        {
            RuleFor(x => x.MessageId).NotEmpty().MaximumLength(500);
        }
    }
}
