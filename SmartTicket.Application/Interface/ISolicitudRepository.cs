using SmartTicket.Domain.Entity;

namespace SmartTicket.Application.Interface;

public interface ISolicitudRepository
{
    Task<long> CreateAsync(Tbl_Solicitud solicitud);
    Task<Tbl_Solicitud?> GetByIdAsync(long solicitudId);
    Task<IEnumerable<Tbl_Solicitud>> GetAllAsync(int? estadoId, int? prioridadId);
    Task CambiarEstadoAsync(long solicitudId, int estadoAnteriorId, int estadoNuevoId, int usuarioId, bool esEstadoFinal);
    Task AsignarAsync(long solicitudId, int usuarioAsignadoId);
    Task<IEnumerable<Tbl_Solicitud>> GetByClienteIdAsync(int clienteId);
}
