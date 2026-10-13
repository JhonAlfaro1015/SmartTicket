using MediatR;
using SmartTicket.Application.Dtos.Solicitudes;

namespace SmartTicket.Application.Features.Solicitudes.Historial;

public record GetHistorialQuery(long SolicitudId, int UsuarioId, string Rol) : IRequest<IEnumerable<HistorialEstadoDto>>;
