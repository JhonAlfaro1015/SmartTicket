using SmartTicket.Domain.Entity;

namespace SmartTicket.Application.Interface;

public interface ISubcategoriaRepository
{
    Task<Tbl_Subcategoria?> GetByCategoriaAndNombreAsync(int categoriaId, string nombre);
}
