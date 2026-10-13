using SmartTicket.Application.Dtos.Solicitudes;

namespace SmartTicket.Application.Interface;

public interface IHistorialRepository
{
    Task<IEnumerable<HistorialEstadoDto>> ListarPorSolicitudAsync(long solicitudId);
}
