using MediatR;
using SmartTicket.Application.Dtos.Solicitudes;

namespace SmartTicket.Application.Features.Solicitudes.Query;

public record GetSolicitudesQuery(int? EstadoId, int? PrioridadId) : IRequest<IEnumerable<SolicitudResponseDto>>;
