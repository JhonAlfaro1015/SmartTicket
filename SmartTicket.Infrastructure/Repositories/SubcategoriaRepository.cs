using Dapper;
using SmartTicket.Application.Interface;
using SmartTicket.Domain.Entity;
using SmartTicket.Infrastructure.Persistence;


namespace SmartTicket.Infrastructure.Repositories;

public class SubcategoriaRepository(DapperContext context) : ISubcategoriaRepository
{
    private readonly DapperContext _context = context;

    public async Task<Tbl_Subcategoria?> GetByCategoriaAndNombreAsync(int categoriaId, string nombre)
    {
        const string sql = @"SELECT SubcategoriaId, CategoriaId, Nombre, Activo FROM Subcategorias WHERE CategoriaId = @CategoriaId AND Nombre = @Nombre";

        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Tbl_Subcategoria>(sql, new { CategoriaId = categoriaId, Nombre = nombre });
    }
}
