using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartTicket.Application.Dtos.Auth.LoginRequestDto;
using SmartTicket.Application.Dtos.Auth;
using SmartTicket.Application.Dtos.Clientes;
using SmartTicket.Application.Features.Clientes.Auth;

namespace SmartTicket.Api.Controllers;

[ApiController]
[Route("api/cliente-auth")]
public class ClienteAuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public ClienteAuthController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpPost("registro")]
    public async Task<ActionResult<AuthResponseDto>> Registro([FromBody] ClienteRegisterDto request)
    {
        var command = _mapper.Map<RegisterClienteCommand>(request);

        try
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] ClienteLoginDto request)
    {
        var command = _mapper.Map<LoginClienteCommand>(request);

        try
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }
}
