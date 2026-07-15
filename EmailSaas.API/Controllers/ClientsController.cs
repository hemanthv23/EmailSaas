using MediatR;
using Microsoft.AspNetCore.Mvc;
using EmailSaas.Application.Features.Clients.Commands.CreateClient;
using EmailSaas.Application.Features.Clients.Queries.GetAllClients;
using EmailSaas.Application.Features.Clients.Queries.GetClientById;

namespace EmailSaas.API.Controllers;

/// <summary>
/// Manages Clients under Applications
/// </summary>
[ApiController]
[Route("api/clients")]
[Produces("application/json")]
public class ClientsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ClientsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new client under an application
    /// </summary>
    [HttpPost("create-client")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Tags("Public")]
    public async Task<IActionResult> Create([FromBody] CreateClientCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result) : BadRequest(result.Errors);
    }

    /// <summary>
    /// Get all clients
    /// </summary>
    [HttpGet("fetch-all-clients")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll()
    {
        var applicationId = (int)HttpContext.Items["ApplicationId"]!;
        var result = await _mediator.Send(new GetAllClientsQuery
        {
            ApplicationId = applicationId
        });
        return result.Succeeded ? Ok(result) : BadRequest(result.Errors);
    }

    /// <summary>
    /// Get client by Id
    /// </summary>
    //[HttpGet("fetch-client/{id:int}")]
    //[ProducesResponseType(StatusCodes.Status200OK)]
    //[ProducesResponseType(StatusCodes.Status404NotFound)]
    //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
    //public async Task<IActionResult> GetById(int id)
    //{
    //    var result = await _mediator.Send(new GetClientByIdQuery { Id = id });
    //    return result.Succeeded ? Ok(result) : BadRequest(result.Errors);
    //}
}