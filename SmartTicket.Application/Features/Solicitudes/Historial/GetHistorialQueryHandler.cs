using MediatR;
using SmartTicket.Application.Common.Exceptions;
using SmartTicket.Application.Common;
using SmartTicket.Application.Dtos.Solicitudes;
using SmartTicket.Application.Interface;


namespace SmartTicket.Application.Features.Solicitudes.Historial;

public class GetHistorialQueryHandler : IRequestHandler<GetHistorialQuery, IEnumerable<HistorialEstadoDto>>
{
    private readonly ISolicitudRepository _solicitudRepository;
    private readonly IHistorialRepository _historialRepository;

    public GetHistorialQueryHandler(ISolicitudRepository solicitudRepository, IHistorialRepository historialRepository)
    {
        _solicitudRepository = solicitudRepository;
        _historialRepository = historialRepository;
    }

    public async Task<IEnumerable<HistorialEstadoDto>> Handle(GetHistorialQuery request, CancellationToken cancellationToken)
    {
        var solicitud = await _solicitudRepository.GetByIdAsync(request.SolicitudId)
            ?? throw new RecursoNoEncontradoException("La solicitud no existe.");

        if (!PermisosSolicitud.PuedeVer(request.Rol, request.UsuarioId, solicitud.ClienteId, solicitud.UsuarioAsignadoId))
            throw new AccesoDenegadoException("No tienes permiso para ver esta solicitud.");

        return await _historialRepository.ListarPorSolicitudAsync(request.SolicitudId);
    }
}
