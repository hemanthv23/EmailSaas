/*
using EmailSaas.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.Features.Webhooks.Commands.UpdateWebhookSubscription
{
    public class UpdateWebhookSubscriptionCommand : IRequest<Result<bool>>
    {
        public int Id { get; set; }
        public string CallbackUrl { get; set; } = string.Empty;
        public List<string> EventTypes { get; set; } = new();
        public byte Status { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
    }
}

*/