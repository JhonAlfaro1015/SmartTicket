using SmartTicket.Application.Dtos.Solicitudes;

namespace SmartTicket.Application.Interface;

public interface ISolicitudLecturaRepository
{
    Task<IEnumerable<SolicitudDetalleDto>> ListarAsync(
        int? estadoId, int? prioridadId, int? clienteId, int? usuarioAsignadoId);

    Task<SolicitudDetalleDto?> ObtenerAsync(long solicitudId);
}
