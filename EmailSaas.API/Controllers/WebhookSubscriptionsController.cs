/*
using EmailSaas.Application.Features.Webhooks.Commands.CreateWebhookSubscription;
using EmailSaas.Application.Features.Webhooks.Commands.DeleteWebhookSubscription;
using EmailSaas.Application.Features.Webhooks.Commands.RegenerateWebhookSecret;
using EmailSaas.Application.Features.Webhooks.Commands.UpdateWebhookSubscription;
using EmailSaas.Application.Features.Webhooks.Queries.GetAllWebhookSubscriptions;
using EmailSaas.Application.Features.Webhooks.Queries.GetWebhookSubscriptionById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EmailSaas.API.Controllers
{
    [ApiController]
    [Route("api/webhook-subscriptions")]
    public class WebhookSubscriptionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WebhookSubscriptionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateWebhookSubscriptionCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateWebhookSubscriptionCommand command, CancellationToken cancellationToken)
        {
            command.Id = id;
            var result = await _mediator.Send(command, cancellationToken);
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, [FromQuery] string updatedBy, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new DeleteWebhookSubscriptionCommand { Id = id, UpdatedBy = updatedBy }, cancellationToken);
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetWebhookSubscriptionByIdQuery { Id = id }, cancellationToken);
            return result.Succeeded ? Ok(result) : NotFound(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int clientId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetAllWebhookSubscriptionsQuery
            {
                ClientId = clientId,
                PageNumber = pageNumber,
                PageSize = pageSize
            }, cancellationToken);
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{id}/regenerate-secret")]
        public async Task<IActionResult> RegenerateSecret(int id, [FromQuery] string updatedBy, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new RegenerateWebhookSecretCommand { Id = id, UpdatedBy = updatedBy }, cancellationToken);
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }
    }
}
*/