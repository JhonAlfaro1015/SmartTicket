using Dapper;
using SmartTicket.Application.Interface;
using SmartTicket.Infrastructure.Persistence;

namespace SmartTicket.Infrastructure.Repositories;

public class ReglaSlaRepository : IReglaSlaRepository
{
    private readonly DapperContext _context;

    public ReglaSlaRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<int?> GetHorasSlaAsync(int? subcategoriaId, int prioridadId)
    {
        const string sql = @"
            SELECT TOP 1 HorasSLA
            FROM dbo.ReglasSLA
            WHERE PrioridadId = @PrioridadId
              AND (SubcategoriaId = @SubcategoriaId OR SubcategoriaId IS NULL)
            ORDER BY CASE WHEN SubcategoriaId IS NULL THEN 1 ELSE 0 END";

        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<int?>(
            sql, new { SubcategoriaId = subcategoriaId, PrioridadId = prioridadId });
    }
}

