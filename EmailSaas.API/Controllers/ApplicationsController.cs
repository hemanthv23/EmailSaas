using EmailSaas.Application.Features.Applications.Commands.CreateApplication;
using EmailSaas.Application.Features.Applications.Commands.RegenerateApiKey;
using EmailSaas.Application.Features.Applications.Queries.GetAllApplications;
using EmailSaas.Application.Features.Applications.Queries.GetAllApplicationsById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EmailSaas.API.Controllers;

/// <summary>
/// Manages EmailSaaS Applications
/// </summary>
[ApiController]
[Route("api/applications")]
[Produces("application/json")]

public class ApplicationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ApplicationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Register a new application — returns ApiKey for authentication
    /// </summary>
    [HttpPost("create-application")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Tags("Public")]
    public async Task<IActionResult> Create([FromBody] CreateApplicationCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result) : BadRequest(result.Errors);
    }

    /// <summary>
    /// Get all registered applications
    /// </summary>
    [HttpGet("fetch-all-applications")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll()
    {
        var applicationId = (int)HttpContext.Items["ApplicationId"]!;
        var result = await _mediator.Send(new GetAllApplicationsQuery
        {
            ApplicationId = applicationId
        });
        return result.Succeeded ? Ok(result) : BadRequest(result.Errors);
    }


    /// <summary>
    /// Regenerate ApiKey — use this if you lost your original ApiKey.
    /// No header required. Only ApplicationCode needed.
    /// Old ApiKey is immediately invalidated.
    /// </summary>
    [HttpPost("regenerate-apikey")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegenerateApiKey(
        [FromBody] RegenerateApiKeyCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result) : BadRequest(result.Errors);
    }
    /// <summary>
    /// Get application by Id
    /// </summary>
    //[HttpGet("fetch-application/{id:int}")]
    //[ProducesResponseType(StatusCodes.Status200OK)]
    //[ProducesResponseType(StatusCodes.Status404NotFound)]
    //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
    //public async Task<IActionResult> GetById(int id)
    //{
    //    var result = await _mediator.Send(new GetApplicationByIdQuery { Id = id });
    //    return result.Succeeded ? Ok(result) : BadRequest(result.Errors);
    //}
}