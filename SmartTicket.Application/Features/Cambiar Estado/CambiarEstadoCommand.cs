using MediatR;

namespace SmartTicket.Application.Features.Cambiar_Estado;

public record CambiarEstadoCommand(long SolicitudId, int NuevoEstadoId, int UsuarioId, string Rol) : IRequest<Unit>;
