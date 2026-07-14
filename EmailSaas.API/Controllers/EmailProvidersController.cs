using EmailSaas.Application.Features.EmailProviders.Commands.CreateEmailProvider;
using EmailSaas.Application.Features.EmailProviders.Commands.UpdateEmailProvider;
using EmailSaas.Application.Features.EmailProviders.Queries.GetAllEmailProviders;
using EmailSaas.Application.Features.EmailProviders.Queries.GetEmailProviderById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EmailSaas.API.Controllers;

/// <summary>
/// Manages Email Provider Configurations (SMTP/SendGrid/SES)
/// </summary>
[ApiController]
[Route("api/email-providers")]
[Produces("application/json")]
public class EmailProvidersController : ControllerBase
{
    private readonly IMediator _mediator;

    public EmailProvidersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create email provider config for a client
    /// </summary>
    [HttpPost("create-email-provider")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    //[Tags("Public")]
    public async Task<IActionResult> Create([FromBody] CreateEmailProviderCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result) : BadRequest(result.Errors);
    }

    /// <summary>
    /// Get all email provider configs
    /// </summary>
    //[HttpGet("fetch-all-email-providers")]
    //[ProducesResponseType(StatusCodes.Status200OK)]
    //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
    //public async Task<IActionResult> GetAll()
    //{
    //    var applicationId = (int)HttpContext.Items["ApplicationId"]!;
    //    var result = await _mediator.Send(new GetAllEmailProvidersQuery
    //    {
    //        ApplicationId = applicationId
    //    });
    //    return result.Succeeded ? Ok(result) : BadRequest(result.Errors);
    //}

    /// <summary>
    /// Get email provider config by Id
    /// </summary>
    //[HttpGet("fetch-email-provider/{id:int}")]
    //[ProducesResponseType(StatusCodes.Status200OK)]
    //[ProducesResponseType(StatusCodes.Status404NotFound)]
    //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
    //public async Task<IActionResult> GetById(int id)
    //{
    //    var result = await _mediator.Send(new GetEmailProviderByIdQuery { Id = id });
    //    return result.Succeeded ? Ok(result) : BadRequest(result.Errors);
    //}

    /// <summary>
    /// Update an existing email provider config
    /// </summary>
    [HttpPut("update-email-provider")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Update([FromBody] UpdateEmailProviderCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result) : BadRequest(result.Errors);
    }
}