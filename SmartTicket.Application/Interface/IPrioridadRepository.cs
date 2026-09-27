using SmartTicket.Domain.Entity;

namespace SmartTicket.Application.Interface;

public interface IPrioridadRepository
{
    Task<Tbl_Prioridad?> GetByNombreAsync(string nombre);
}
