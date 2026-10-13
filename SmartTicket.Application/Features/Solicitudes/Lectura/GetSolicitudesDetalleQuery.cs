using MediatR;
using SmartTicket.Application.Dtos.Solicitudes;

namespace SmartTicket.Application.Features.Solicitudes.Lectura;

public record GetSolicitudesDetalleQuery(int? EstadoId, int? PrioridadId, int UsuarioId, string Rol) : IRequest<IEnumerable<SolicitudDetalleDto>>;
