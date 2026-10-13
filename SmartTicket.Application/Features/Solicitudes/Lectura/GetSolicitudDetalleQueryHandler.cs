using MediatR;
using SmartTicket.Application.Common.Exceptions;
using SmartTicket.Application.Common;
using SmartTicket.Application.Dtos.Solicitudes;
using SmartTicket.Application.Interface;

namespace SmartTicket.Application.Features.Solicitudes.Lectura;

public class GetSolicitudDetalleQueryHandler(
    ISolicitudLecturaRepository lecturaRepository,
    IEstadoTransicionRepository transicionRepository) : IRequestHandler<GetSolicitudDetalleQuery, SolicitudDetalleDto>
{
    private readonly ISolicitudLecturaRepository _lecturaRepository = lecturaRepository;
    private readonly IEstadoTransicionRepository _transicionRepository = transicionRepository;

    public async Task<SolicitudDetalleDto> Handle(GetSolicitudDetalleQuery request, CancellationToken cancellationToken)
    {
        var detalle = await _lecturaRepository.ObtenerAsync(request.SolicitudId)
            ?? throw new RecursoNoEncontradoException("La solicitud no existe.");

        if (!PermisosSolicitud.PuedeVer(request.Rol, request.UsuarioId, detalle.ClienteId, detalle.UsuarioAsignadoId))
            throw new AccesoDenegadoException("No tienes permiso para ver esta solicitud.");

        var destinos = await _transicionRepository.GetDestinosAsync(detalle.EstadoId);
        CapacidadesSolicitud.Aplicar(detalle, request.Rol, request.UsuarioId, destinos);

        return detalle;
    }
}
