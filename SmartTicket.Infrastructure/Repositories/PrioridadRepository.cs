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
}
