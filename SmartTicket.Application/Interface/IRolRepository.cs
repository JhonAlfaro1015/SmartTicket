using SmartTicket.Domain.Entity;

namespace SmartTicket.Application.Interface;

public interface IRolRepository
{
    Task<Tbl_Rol?> GetByIdAsync(int rolId);
    Task<Tbl_Rol?> GetByNombreAsync(string nombre);
}