using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTicket.Application.Dtos.Clientes;
using SmartTicket.Application.Features.Clientes.Create;
using SmartTicket.Application.Features.Clientes.Query;

namespace SmartTicket.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClientesController(IMediator mediator, IMapper mapper) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly IMapper _mapper = mapper;

    [HttpPost]
    public async Task<ActionResult<ClienteResponseDto>> Create([FromBody] CreateClienteDto request)
    {
        var command = _mapper.Map<CreateClienteCommand>(request);
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.ClienteId }, result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ClienteResponseDto>> GetById(int id)
    {
        var result = await _mediator.Send(new GetClienteByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }
}
