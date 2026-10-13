using MediatR;
using SmartTicket.Application.Common;
using SmartTicket.Application.Dtos.Solicitudes;
using SmartTicket.Application.Interface;

namespace SmartTicket.Application.Features.Solicitudes.Lectura;

public class GetSolicitudesDetalleQueryHandler
    : IRequestHandler<GetSolicitudesDetalleQuery, IEnumerable<SolicitudDetalleDto>>
{
    private readonly ISolicitudLecturaRepository _lecturaRepository;
    private readonly IEstadoTransicionRepository _transicionRepository;

    public GetSolicitudesDetalleQueryHandler(
        ISolicitudLecturaRepository lecturaRepository,
        IEstadoTransicionRepository transicionRepository)
    {
        _lecturaRepository = lecturaRepository;
        _transicionRepository = transicionRepository;
    }

    public async Task<IEnumerable<SolicitudDetalleDto>> Handle(
        GetSolicitudesDetalleQuery request, CancellationToken cancellationToken)
    {
        int? clienteId = request.Rol == RolesSistema.Cliente ? request.UsuarioId : null;
        int? asignadoA = request.Rol == RolesSistema.Agente ? request.UsuarioId : null;

        var solicitudes = (await _lecturaRepository.ListarAsync(
            request.EstadoId, request.PrioridadId, clienteId, asignadoA)).ToList();

        // Una consulta por estado distinto (máximo cinco), no una por solicitud
        var destinosPorEstado = new Dictionary<int, List<EstadoSiguienteDto>>();
        foreach (var estadoId in solicitudes.Select(s => s.EstadoId).Distinct())
            destinosPorEstado[estadoId] = (await _transicionRepository.GetDestinosAsync(estadoId)).ToList();

        foreach (var solicitud in solicitudes)
            CapacidadesSolicitud.Aplicar(solicitud, request.Rol, request.UsuarioId, destinosPorEstado[solicitud.EstadoId]);

        return solicitudes;
    }
}
