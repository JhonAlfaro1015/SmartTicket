using Dapper;
using SmartTicket.Application.Interface;
using SmartTicket.Domain.Entity;
using SmartTicket.Infrastructure.Persistence;


namespace SmartTicket.Infrastructure.Repositories;

public class PrioridadRepository(DapperContext context) : IPrioridadRepository
{
    private readonly DapperContext _context = context;

    public async Task<Tbl_Prioridad?> GetByNombreAsync(string nombre)
    {
        const string sql = "SELECT PrioridadId, Nombre, NivelUrgencia FROM Prioridades WHERE Nombre = @Nombre";
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Tbl_Prioridad>(sql, new { Nombre = nombre });
    }

    public async Task<IEnumerable<Tbl_Prioridad>> GetAllAsync()
    {
        const string sql = "SELECT PrioridadId, Nombre, NivelUrgencia FROM dbo.Prioridades ORDER BY NivelUrgencia";
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<Tbl_Prioridad>(sql);
    }
}
