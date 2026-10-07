using SmartTicket.Domain.Entity;

namespace SmartTicket.Application.Interface;

public interface IEstadoRepository
{
    Task<Tbl_Estado?> GetByIdAsync(int estadoId);
    Task<Tbl_Estado?> GetByNombreAsync(string nombre);
    Task<IEnumerable<Tbl_Estado>> GetAllAsync();
}
