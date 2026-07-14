using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.Features.Tracking.Commands.RecordEmailClick
{
    public class RecordEmailClickCommandValidator : AbstractValidator<RecordEmailClickCommand>
    {
        public RecordEmailClickCommandValidator()
        {
            RuleFor(x => x.MessageId).NotEmpty().MaximumLength(500);
            RuleFor(x => x.OriginalUrl).NotEmpty().MaximumLength(2000);
        }
    }
}
