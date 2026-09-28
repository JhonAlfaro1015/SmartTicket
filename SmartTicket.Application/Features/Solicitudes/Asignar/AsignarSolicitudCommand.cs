using MediatR;

namespace SmartTicket.Application.Features.Solicitudes.Asignar;

public record AsignarSolicitudCommand(long SolicitudId, int UsuarioAsignadoId) : IRequest<Unit>;
