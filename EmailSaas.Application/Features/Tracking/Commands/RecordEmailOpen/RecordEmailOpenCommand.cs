using EmailSaas.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.Features.Tracking.Commands.RecordEmailOpen
{
    public class RecordEmailOpenCommand : IRequest<Result<bool>>
    {
        public string MessageId { get; set; } = string.Empty;
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
    }
}
