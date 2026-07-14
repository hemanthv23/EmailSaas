using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.Features.Tracking.Commands.RecordEmailDelivered
{
    public class RecordEmailDeliveredCommandValidator : AbstractValidator<RecordEmailDeliveredCommand>
    {
        public RecordEmailDeliveredCommandValidator()
        {
            RuleFor(x => x.MessageId).NotEmpty().MaximumLength(500);
        }
    }
}
