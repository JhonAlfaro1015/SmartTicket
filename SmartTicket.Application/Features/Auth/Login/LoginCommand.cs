using MediatR;
using SmartTicket.Application.Dtos.Auth.LoginRequestDto;

namespace SmartTicket.Application.Features.Auth.Login
{
    public record LoginCommand(string Email, string Password) : IRequest<AuthResponseDto>; 
}
