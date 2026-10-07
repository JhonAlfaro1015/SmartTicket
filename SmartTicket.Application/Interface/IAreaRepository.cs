using SmartTicket.Domain.Entity;

namespace SmartTicket.Application.Interface;

public interface IAreaRepository
{
    Task<Tbl_Area?> GetByNombreAsync(string nombre);
    Task<IEnumerable<Tbl_Area>> GetAllAsync();
}
