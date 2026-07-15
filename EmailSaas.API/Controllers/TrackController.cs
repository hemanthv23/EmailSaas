using EmailSaas.Application.Features.Tracking.Commands.RecordEmailClick;
using EmailSaas.Application.Features.Tracking.Commands.RecordEmailOpen;
using EmailSaas.Application.Features.Tracking.Commands.RecordEmailDelivered;
using EmailSaas.Application.Features.Tracking.Commands.RecordEmailBounced;
using EmailSaas.Application.Features.Tracking.Commands.RecordEmailFailed;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
namespace EmailSaas.API.Controllers
{
    [ApiController]
    [Route("api/track")]
    public class TrackController : ControllerBase
    {
        private readonly IMediator _mediator;
        // Valid 1x1 transparent GIF
        private static readonly byte[] TransparentPixel = Convert.FromBase64String(
            "R0lGODlhAQABAIAAAAAAAP///yH5BAEAAAAALAAAAAABAAEAAAIBRAA7");

        public TrackController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("open/{messageId}")]
        public async Task<IActionResult> TrackOpen(string messageId, CancellationToken cancellationToken)
        {
            var command = new RecordEmailOpenCommand
            {
                MessageId = messageId,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = Request.Headers["User-Agent"].ToString()
            };

            await _mediator.Send(command, cancellationToken);

            Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, max-age=0";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            return File(TransparentPixel, "image/gif");
        }
        [HttpGet("click/{messageId}")]
        public async Task<IActionResult> TrackClick(string messageId, [FromQuery] string url, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(url))
                return BadRequest("Missing target URL.");
            // Note: ASP.NET Core automatically URL-decodes the [FromQuery] parameter.
            // Using Uri.UnescapeDataString again can break URLs that legitimately contain encoded characters (like spaces encoded as %20).
            var command = new RecordEmailClickCommand
            {
                MessageId = messageId,
                OriginalUrl = url,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = Request.Headers["User-Agent"].ToString()
            };
            var result = await _mediator.Send(command, cancellationToken);
            // ✅ Fixed — Succeeded, not IsSuccess
            var redirectUrl = result.Succeeded ? result.Data : url;
            return Redirect(redirectUrl!);
        }

        // ─── New: manual/provider-callback delivery confirmation ───
        [HttpPost("delivered")]
        public async Task<IActionResult> TrackDelivered([FromBody] RecordEmailDeliveredCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        // ─── New: bounce recording (called by IMAP listener, or manually for testing) ───
        [HttpPost("bounced")]
        public async Task<IActionResult> TrackBounced([FromBody] RecordEmailBouncedCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        // ─── New: failure recording ───
        [HttpPost("failed")]
        public async Task<IActionResult> TrackFailed([FromBody] RecordEmailFailedCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        // ─── Automatic Provider Webhook Receiver ───
        [HttpPost("events")]
        public async Task<IActionResult> TrackProviderWebhook([FromBody] JsonElement[] events, CancellationToken cancellationToken)
        {
            foreach (var evt in events)
            {
                if (!evt.TryGetProperty("event", out var eventProperty))
                    continue;

                var eventType = eventProperty.GetString()?.ToLower();

                // Get our internal message ID from custom arguments (try both snake_case and camelCase)
                if (!evt.TryGetProperty("message_id", out var messageIdProp) && 
                    !evt.TryGetProperty("messageId", out messageIdProp))
                {
                    continue;
                }

                var messageId = messageIdProp.GetString();
                if (string.IsNullOrEmpty(messageId)) continue;

                if (eventType == "delivered")
                {
                    DateTime? deliveredAt = null;
                    if (evt.TryGetProperty("timestamp", out var timestampProp))
                    {
                        if (timestampProp.ValueKind == JsonValueKind.Number && timestampProp.TryGetInt64(out var ts))
                        {
                            deliveredAt = DateTimeOffset.FromUnixTimeSeconds(ts).UtcDateTime;
                        }
                    }

                    var command = new RecordEmailDeliveredCommand
                    {
                        MessageId = messageId,
                        ProviderResponse = evt.GetRawText(),
                        DeliveredAt = deliveredAt
                    };
                    await _mediator.Send(command, cancellationToken);
                }
                else if (eventType == "bounce" || eventType == "dropped")
                {
                    var reason = (evt.TryGetProperty("reason", out var r) ? r.GetString() : null) ?? "Bounced/Dropped by Provider";
                    var command = new RecordEmailBouncedCommand
                    {
                        MessageId = messageId,
                        BounceReason = reason
                    };
                    await _mediator.Send(command, cancellationToken);
                }
            }

            return Ok();
        }
    }
}