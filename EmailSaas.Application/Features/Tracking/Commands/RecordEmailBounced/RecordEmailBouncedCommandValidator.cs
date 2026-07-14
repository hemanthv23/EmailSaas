using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.Features.Tracking.Commands.RecordEmailBounced
{
    public class RecordEmailBouncedCommandValidator : AbstractValidator<RecordEmailBouncedCommand>
    {
        public RecordEmailBouncedCommandValidator()
        {
            RuleFor(x => x.MessageId).NotEmpty().MaximumLength(500);
            RuleFor(x => x.BounceReason).NotEmpty().MaximumLength(1000);
        }
    }
}
