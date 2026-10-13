using MediatR;
using SmartTicket.Application.Dtos.Solicitudes;

namespace SmartTicket.Application.Features.Solicitudes.Lectura;

public record GetSolicitudDetalleQuery(long SolicitudId, int UsuarioId, string Rol) : IRequest<SolicitudDetalleDto>;
