using Dapper;
using SmartTicket.Application.Interface;
using SmartTicket.Domain.Entity;
using SmartTicket.Infrastructure.Persistence;


namespace SmartTicket.Infrastructure.Repositories;

public class AreaRepository(DapperContext context) : IAreaRepository
{
    private readonly DapperContext _context = context;

    public async Task<Tbl_Area?> GetByNombreAsync(string nombre)
    {
        const string sql = "SELECT AreaId, Nombre, Activo FROM dbo.Areas WHERE Nombre = @Nombre";
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Tbl_Area>(sql, new { Nombre = nombre });
    }
}
