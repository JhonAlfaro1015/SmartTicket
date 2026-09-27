using MediatR;
using SmartTicket.Application.Dtos.Solicitudes;

namespace SmartTicket.Application.Features.Solicitudes.Query;

public record GetSolicitudByIdQuery(long SolicitudId) : IRequest<SolicitudResponseDto?>;
