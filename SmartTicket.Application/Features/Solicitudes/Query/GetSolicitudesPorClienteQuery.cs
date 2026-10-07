using MediatR;
using SmartTicket.Application.Dtos.Solicitudes;

namespace SmartTicket.Application.Features.Solicitudes.Query;

public record GetSolicitudesPorClienteQuery(int ClienteId) : IRequest<IEnumerable<SolicitudResponseDto>>;


