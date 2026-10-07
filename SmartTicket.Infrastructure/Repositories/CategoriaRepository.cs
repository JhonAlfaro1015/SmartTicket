using Dapper;
using SmartTicket.Application.Interface;
using SmartTicket.Domain.Entity;
using SmartTicket.Infrastructure.Persistence;

namespace SmartTicket.Infrastructure.Repositories;

public class CategoriaRepository(DapperContext context) : ICategoriaRepository
{
    private readonly DapperContext _context = context;

    public async Task<Tbl_Categoria?> GetByNombreAsync(string nombre)
    {
        const string sql = "SELECT CategoriaId, Nombre, Activo FROM Categorias WHERE Nombre = @Nombre";
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Tbl_Categoria>(sql, new { Nombre = nombre });
    }

    public async Task<IEnumerable<Tbl_Categoria>> GetAllAsync()
    {
        const string sql = "SELECT CategoriaId, Nombre, Activo FROM dbo.Categorias WHERE Activo = 1 ORDER BY Nombre";
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<Tbl_Categoria>(sql);
    }
}
