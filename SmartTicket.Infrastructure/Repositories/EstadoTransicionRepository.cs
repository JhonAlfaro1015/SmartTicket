using Dapper;
using SmartTicket.Application.Dtos.Solicitudes;
using SmartTicket.Application.Interface;
using SmartTicket.Infrastructure.Persistence;

namespace SmartTicket.Infrastructure.Repositories;

public class EstadoTransicionRepository(DapperContext context) : IEstadoTransicionRepository
{
    private readonly DapperContext _context = context;

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

    public async Task<IEnumerable<EstadoSiguienteDto>> GetDestinosAsync(int estadoOrigenId)
    {
        const string sql = @"
            SELECT e.EstadoId, e.Nombre, e.EsFinal
            FROM dbo.EstadoTransiciones t
            JOIN dbo.Estados e ON e.EstadoId = t.EstadoDestinoId
            WHERE t.EstadoOrigenId = @EstadoOrigenId
            ORDER BY e.Orden, e.EstadoId";

        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<EstadoSiguienteDto>(sql, new { EstadoOrigenId = estadoOrigenId });
    }
}
