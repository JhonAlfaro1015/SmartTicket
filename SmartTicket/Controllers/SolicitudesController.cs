using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTicket.Application.Dtos.Solicitudes;
using SmartTicket.Application.Features.Cambiar_Estado;
using SmartTicket.Application.Features.Solicitudes.Asignar;
using SmartTicket.Application.Features.Solicitudes.Create;
using SmartTicket.Application.Features.Solicitudes.Query;
using System.Security.Claims;

namespace SmartTicket.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SolicitudesController(IMediator mediator, IMapper mapper) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly IMapper _mapper = mapper;

    [HttpPost]
    [Authorize(Roles = "Cliente")] // solo el cliente autenticado crea su propia solicitud
    public async Task<ActionResult<SolicitudResponseDto>> Create([FromBody] CreateSolicitudDto request)
    {
        var clienteId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var command = new CreateSolicitudCommand(clienteId, request.TextoOriginal);

        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.SolicitudId }, result);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Administrador,Agente,Supervisor")]
    public async Task<ActionResult<SolicitudResponseDto>> GetById(long id)
    {
        var result = await _mediator.Send(new GetSolicitudByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet]
    [Authorize(Roles = "Administrador,Agente,Supervisor")]
    public async Task<ActionResult<IEnumerable<SolicitudResponseDto>>> GetAll([FromQuery] int? estadoId, [FromQuery] int? prioridadId)
    {
        var result = await _mediator.Send(new GetSolicitudesQuery(estadoId, prioridadId));
        return Ok(result);
    }

    [HttpPut("{id}/estado")]
    [Authorize(Roles = "Administrador,Agente,Supervisor")]
    public async Task<IActionResult> CambiarEstado(long id, [FromBody] CambiarEstadoDto request)
    {
        try
        {
            await _mediator.Send(new CambiarEstadoCommand(id, request.NuevoEstadoId, request.UsuarioId));
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}/asignar")]
    [Authorize(Roles = "Administrador,Supervisor")]
    public async Task<IActionResult> Asignar(long id, [FromBody] AsignarSolicitudDto request)
    {
        try
        {
            await _mediator.Send(new AsignarSolicitudCommand(id, request.UsuarioAsignadoId));
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
