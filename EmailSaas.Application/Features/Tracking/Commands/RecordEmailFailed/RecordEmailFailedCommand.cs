using EmailSaas.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.Features.Tracking.Commands.RecordEmailFailed
{
    public class RecordEmailFailedCommand : IRequest<Result<bool>>
    {
        public string MessageID { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
    }
}