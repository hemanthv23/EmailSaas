using EmailSaas.Application.Common.Models;
using EmailSaas.Application.DTOs.Webhook;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.Features.Webhooks.Commands.CreateWebhookSubscription
{
    public class CreateWebhookSubscriptionCommand : WebhookSubscriptionRequestDto, IRequest<Result<WebhookSubscriptionResponseDto>>
    {
    }
}
