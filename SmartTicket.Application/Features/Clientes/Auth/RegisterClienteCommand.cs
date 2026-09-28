using MediatR;
using SmartTicket.Application.Dtos.Auth.LoginRequestDto;

namespace SmartTicket.Application.Features.Clientes.Auth;

public record RegisterClienteCommand(string Nombre, string Email, string? Telefono, string Password) : IRequest<AuthResponseDto>;
