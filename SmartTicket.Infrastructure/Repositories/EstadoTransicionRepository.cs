using Dapper;
using SmartTicket.Application.Interface;
using SmartTicket.Infrastructure.Persistence;

namespace SmartTicket.Infrastructure.Repositories;

public class EstadoTransicionRepository : IEstadoTransicionRepository
{
    private readonly DapperContext _context;

    public EstadoTransicionRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<bool> EsPermitidaAsync(int estadoOrigenId, int estadoDestinoId)
    {
        const string sql = @"
            SELECT COUNT(1)
            FROM dbo.EstadoTransiciones
            WHERE EstadoOrigenId = @EstadoOrigenId AND EstadoDestinoId = @EstadoDestinoId";

        using var connection = _context.CreateConnection();
        var cantidad = await connection.ExecuteScalarAsync<int>(
            sql, new { EstadoOrigenId = estadoOrigenId, EstadoDestinoId = estadoDestinoId });

        return cantidad > 0;
    }
}
