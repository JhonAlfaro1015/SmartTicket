using MediatR;
using SmartTicket.Application.Dtos.Clientes;

namespace SmartTicket.Application.Features.Clientes.Query;

public record GetClienteByIdQuery(int ClienteId) : IRequest<ClienteResponseDto?>;
