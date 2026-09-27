using MediatR;
using SmartTicket.Application.Dtos.Solicitudes;

namespace SmartTicket.Application.Features.Solicitudes.Create;

public record CreateSolicitudCommand(int ClienteId, string TextoOriginal) : IRequest<SolicitudResponseDto>;
