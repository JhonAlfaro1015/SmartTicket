using MediatR;
using SmartTicket.Application.Dtos.Clientes;

namespace SmartTicket.Application.Features.Clientes.Create;

public record CreateClienteCommand(string Nombre, string? Email, string? Telefono) : IRequest<ClienteResponseDto>;
