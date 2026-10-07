using SmartTicket.Domain.Entity;

namespace SmartTicket.Application.Interface;

public interface ICategoriaRepository
{
    Task<Tbl_Categoria?> GetByNombreAsync(string nombre);
    Task<IEnumerable<Tbl_Categoria>> GetAllAsync();
}
