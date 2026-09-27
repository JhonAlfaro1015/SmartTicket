using MediatR;
using SmartTicket.Application.Dtos.Auth.LoginRequestDto;

public record RegisterCommand(string Nombre, string Email, string Password, string RolNombre) : IRequest<AuthResponseDto>;