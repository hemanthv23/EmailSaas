using EmailSaas.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.Features.Tracking.Commands.RecordEmailDelivered
{
    public class RecordEmailDeliveredCommand : IRequest<Result<bool>>
    {
        public string MessageId { get; set; } = string.Empty;
        public string? ProviderResponse { get; set; }
    }
}
