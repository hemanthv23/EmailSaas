using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using EmailSaas.Application.Features.SendEmail.Commands;
namespace EmailSaas.API.Controllers;
/// <summary>
/// Core API — Send emails using stored templates
/// </summary>
[ApiController]
[Route("api/send-email")]
[Produces("application/json")]
[Tags("Public")]
public class SendEmailController : ControllerBase
{
    private readonly IMediator _mediator;
    public SendEmailController(IMediator mediator)
    {
        _mediator = mediator;
    }
    /// <summary>
    /// Send an email using a stored template with dynamic parameters.
    /// This is the MAIN API that external applications call.
    /// </summary>
    [HttpPost("send")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Send([FromBody] SendEmailCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result) : BadRequest(result.Errors);
    }
}