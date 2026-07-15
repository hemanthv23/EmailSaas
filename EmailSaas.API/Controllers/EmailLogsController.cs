using MediatR;
using Microsoft.AspNetCore.Mvc;
using EmailSaas.Application.Features.EmailLogs.Queries.GetAllEmailLogs;
using EmailSaas.Application.Features.EmailLogs.Queries.GetEmailLogById;

namespace EmailSaas.API.Controllers;

/// <summary>
/// View email sending history and logs
/// </summary>
[ApiController]
[Route("api/email-logs")]
[Produces("application/json")]
public class EmailLogsController : ControllerBase
{
    private readonly IMediator _mediator;

    public EmailLogsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all email logs — shows Sent/Failed/Pending history
    /// </summary>
    [HttpGet("fetch-all-email-logs")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Tags("Public")]
    public async Task<IActionResult> GetAll()
    {
        var applicationId = (int)HttpContext.Items["ApplicationId"]!;
        var result = await _mediator.Send(new GetAllEmailLogsQuery
        {
            ApplicationId = applicationId
        });
        return result.Succeeded ? Ok(result) : BadRequest(result.Errors);
    }

    /// <summary>
    /// Get email log by Id
    /// </summary>
    //[HttpGet("fetch-email-log/{id:int}")]
    //[ProducesResponseType(StatusCodes.Status200OK)]
    //[ProducesResponseType(StatusCodes.Status404NotFound)]
    //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
    //public async Task<IActionResult> GetById(int id)
    //{
    //    var result = await _mediator.Send(new GetEmailLogByIdQuery { Id = id });
    //    return result.Succeeded ? Ok(result) : BadRequest(result.Errors);
    //}
}