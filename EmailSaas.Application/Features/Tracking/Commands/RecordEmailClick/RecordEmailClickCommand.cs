using EmailSaas.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.Features.Tracking.Commands.RecordEmailClick
{
    public class RecordEmailClickCommand : IRequest<Result<string>> // returns original URL to redirect to
    {
        public string MessageId { get; set; } = string.Empty;
        public string OriginalUrl { get; set; } = string.Empty;
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
    }
}
