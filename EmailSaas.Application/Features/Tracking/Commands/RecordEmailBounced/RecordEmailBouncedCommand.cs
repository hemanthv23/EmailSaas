using EmailSaas.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.Features.Tracking.Commands.RecordEmailBounced
{
    public class RecordEmailBouncedCommand : IRequest<Result<bool>>
    {
        public string MessageID { get; set; } = string.Empty;
        public string BounceReason { get; set; } = string.Empty;
        public bool IsHardBounce { get; set; } = true; // hard = permanent failure, soft = temporary
    }
}