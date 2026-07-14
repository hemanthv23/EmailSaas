using EmailSaas.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.Features.Webhooks.Commands.RegenerateWebhookSecret
{
    public class RegenerateWebhookSecretCommand : IRequest<Result<string>> // returns new plain secret, once
    {
        public int Id { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
    }
}
