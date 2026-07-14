using EmailSaas.Application.Features.Tracking.Commands.RecordEmailClick;
using EmailSaas.Application.Features.Tracking.Commands.RecordEmailOpen;
using EmailSaas.Application.Features.Tracking.Commands.RecordEmailDelivered;
using EmailSaas.Application.Features.Tracking.Commands.RecordEmailBounced;
using EmailSaas.Application.Features.Tracking.Commands.RecordEmailFailed;
using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace EmailSaas.API.Controllers
{
    [ApiController]
    [Route("api/track")]
    public class TrackController : ControllerBase
    {
        private readonly IMediator _mediator;
        // 1x1 transparent GIF — always returned regardless of DB outcome
        private static readonly byte[] TransparentPixel = Convert.FromBase64String(
            "R0lGODlhAQABAIAAAAAAAP///yH5BAEAAAAALAAAAAABAAEAAAIBTAA7");
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
            return File(TransparentPixel, "image/gif");
        }
        [HttpGet("click/{messageId}")]
        public async Task<IActionResult> TrackClick(string messageId, [FromQuery] string url, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(url))
                return BadRequest("Missing target URL.");
            var decodedUrl = Uri.UnescapeDataString(url);
            var command = new RecordEmailClickCommand
            {
                MessageId = messageId,
                OriginalUrl = decodedUrl,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = Request.Headers["User-Agent"].ToString()
            };
            var result = await _mediator.Send(command, cancellationToken);
            // ✅ Fixed — Succeeded, not IsSuccess
            var redirectUrl = result.Succeeded ? result.Data : decodedUrl;
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
    }
}