using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTicket.Application.Common;
using SmartTicket.Application.Dtos.Comentarios.Agregar;
using SmartTicket.Application.Dtos.Comentarios;
using SmartTicket.Application.Dtos.Solicitudes;
using SmartTicket.Application.Features.Cambiar_Estado;
using SmartTicket.Application.Features.Comentarios.Listar;
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
    [Authorize(Roles = RolesSistema.Personal)]
    public async Task<IActionResult> CambiarEstado(long id, [FromBody] CambiarEstadoDto request)
    {
        var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var rol = User.FindFirstValue(ClaimTypes.Role)!;

        await _mediator.Send(new CambiarEstadoCommand(id, request.NuevoEstadoId, usuarioId, rol));
        return NoContent();
    }

    [HttpPut("{id}/asignar")]
    [Authorize(Roles = RolesSistema.Gestion)]
    public async Task<IActionResult> Asignar(long id, [FromBody] AsignarSolicitudDto request)
    {
        await _mediator.Send(new AsignarSolicitudCommand(id, request.UsuarioAsignadoId));
        return NoContent();
    }

    [HttpGet("mias")]
    [Authorize(Roles = "Cliente")]
    public async Task<ActionResult<IEnumerable<SolicitudResponseDto>>> Mias()
    {
        var clienteId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _mediator.Send(new GetSolicitudesPorClienteQuery(clienteId));
        return Ok(result);
    }

    [HttpGet("{id}/comentarios")]
    public async Task<ActionResult<IEnumerable<ComentarioDto>>> GetComentarios(long id)
    {
        var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var rol = User.FindFirstValue(ClaimTypes.Role)!;

        var comentarios = await _mediator.Send(new GetComentariosQuery(id, usuarioId, rol));
        return Ok(comentarios);
    }

    [HttpPost("{id}/comentarios")]
    public async Task<ActionResult<ComentarioDto>> AgregarComentario(long id, [FromBody] CrearComentarioDto request)
    {
        var autorId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var rol = User.FindFirstValue(ClaimTypes.Role)!;

        var comentario = await _mediator.Send(new AgregarComentarioCommand(id, request.Texto, autorId, rol));
        return Ok(comentario);
    }
}
