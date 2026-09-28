using MediatR;
using SmartTicket.Application.Dtos.Auth.LoginRequestDto;

namespace SmartTicket.Application.Dtos.Auth;

public record LoginClienteCommand(string Email, string Password) : IRequest<AuthResponseDto>;
