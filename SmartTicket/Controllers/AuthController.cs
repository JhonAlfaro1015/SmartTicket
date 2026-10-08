using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTicket.Application.Common;
using SmartTicket.Application.Dtos.Auth;
using SmartTicket.Application.Dtos.Auth.LoginRequestDto;
using SmartTicket.Application.Features.Auth.Login;

namespace SmartTicket.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IMediator mediator, IMapper mapper) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly IMapper _mapper = mapper;

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto request)
        {
            var command = _mapper.Map<LoginCommand>(request);

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

        [HttpPost("register")]
        [Authorize(Roles = RolesSistema.Administrador)]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterRequestDto request)
        {
            var command = _mapper.Map<RegisterCommand>(request);

            try
            {
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
