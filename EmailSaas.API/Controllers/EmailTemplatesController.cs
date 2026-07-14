using EmailSaas.Application.Features.EmailTemplates.Commands.CreateEmailTemplate;
using EmailSaas.Application.Features.EmailTemplates.Commands.UpdateEmailTemplate;
using EmailSaas.Application.Features.EmailTemplates.Queries.GetAllEmailTemplates;
using EmailSaas.Application.Features.EmailTemplates.Queries.GetEmailTemplateById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EmailSaas.API.Controllers;

/// <summary>
/// Manages Email Templates with dynamic placeholders
/// </summary>
[ApiController]
[Route("api/email-templates")]
[Produces("application/json")]
public class EmailTemplatesController : ControllerBase
{
    private readonly IMediator _mediator;

    public EmailTemplatesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new email template with HTML body and placeholders
    /// </summary>
    [HttpPost("create-email-template")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    //[Tags("Public")]
    public async Task<IActionResult> Create([FromBody] CreateEmailTemplateCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result) : BadRequest(result.Errors);
    }

    /// <summary>
    /// Get all email templates for the authenticated application
    /// </summary>
    [HttpGet("fetch-all-email-templates")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll()
    {
        var applicationId = (int)HttpContext.Items["ApplicationId"]!;
        var result = await _mediator.Send(new GetAllEmailTemplatesQuery
        {
            ApplicationId = applicationId
        });
        return result.Succeeded ? Ok(result) : BadRequest(result.Errors);
    }

    /// <summary>
    /// Get email template by Id
    /// </summary>
    //[HttpGet("fetch-email-template/{id:int}")]
    //[ProducesResponseType(StatusCodes.Status200OK)]
    //[ProducesResponseType(StatusCodes.Status404NotFound)]
    //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
    //public async Task<IActionResult> GetById(int id)
    //{
    //    var result = await _mediator.Send(new GetEmailTemplateByIdQuery { Id = id });
    //    return result.Succeeded ? Ok(result) : BadRequest(result.Errors);
    //}


    /// <summary>
    /// Update an existing email template
    /// </summary>
    [HttpPut("update-email-template")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Update([FromBody] UpdateEmailTemplateCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result) : BadRequest(result.Errors);
    }
}