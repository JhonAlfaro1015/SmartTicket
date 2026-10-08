using Dapper;
using SmartTicket.Application.Interface;
using SmartTicket.Domain.Entity;
using SmartTicket.Infrastructure.Persistence;

namespace SmartTicket.Infrastructure.Repositories;

public class SolicitudRepository(DapperContext context) : ISolicitudRepository
{
    private readonly DapperContext _context = context;

    public async Task<long> CreateAsync(Tbl_Solicitud solicitud)
    {
        const string sql = @"INSERT INTO Solicitudes (ClienteId, CategoriaId, SubcategoriaId, PrioridadId, AreaId, EstadoId, UsuarioAsignadoId, TextoOriginal, Puntuacion, FechaCreacion, FechaLimiteSLA, FechaCierre)
                            VALUES
                            (@ClienteId, @CategoriaId, @SubcategoriaId, @PrioridadId, @AreaId, @EstadoId,
                            @UsuarioAsignadoId, @TextoOriginal, @Puntuacion, @FechaCreacion, @FechaLimiteSLA, @FechaCierre);
                            SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

        using var connection = _context.CreateConnection();
        return await connection.ExecuteScalarAsync<long>(sql, solicitud);
    }

    public async Task<Tbl_Solicitud?> GetByIdAsync(long solicitudId)
    {
        const string sql = @"SELECT SolicitudId, ClienteId, CategoriaId, SubcategoriaId, PrioridadId, AreaId, EstadoId, UsuarioAsignadoId, TextoOriginal, Puntuacion, FechaCreacion, FechaLimiteSLA, FechaCierre
                            FROM Solicitudes
                            WHERE SolicitudId = @SolicitudId";

        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Tbl_Solicitud>(sql, new { SolicitudId = solicitudId });
    }

    public async Task<IEnumerable<Tbl_Solicitud>> GetAllAsync(int? estadoId, int? prioridadId)
    {
        var sql = @"SELECT SolicitudId, ClienteId, CategoriaId, SubcategoriaId, PrioridadId, AreaId, EstadoId,UsuarioAsignadoId, TextoOriginal, Puntuacion, FechaCreacion, FechaLimiteSLA, FechaCierre
                    FROM Solicitudes
                    WHERE 1 = 1";

        if (estadoId.HasValue) sql += " AND EstadoId = @EstadoId";
        if (prioridadId.HasValue) sql += " AND PrioridadId = @PrioridadId";
        sql += " ORDER BY FechaCreacion DESC";

        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<Tbl_Solicitud>(sql, new { EstadoId = estadoId, PrioridadId = prioridadId });
    }

    public async Task CambiarEstadoAsync(long solicitudId, int estadoAnteriorId, int estadoNuevoId, int usuarioId, bool esEstadoFinal)
    {
        using var connection = (Microsoft.Data.SqlClient.SqlConnection)_context.CreateConnection();
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        const string updateSql = @"
            UPDATE dbo.Solicitudes
            SET EstadoId = @EstadoNuevoId,
                FechaCierre = CASE WHEN @EsFinal = 1 THEN SYSUTCDATETIME() ELSE FechaCierre END
            WHERE SolicitudId = @SolicitudId AND EstadoId = @EstadoAnteriorId";

        var filasAfectadas = await connection.ExecuteAsync(updateSql, new
        {
            EstadoNuevoId = estadoNuevoId,
            EstadoAnteriorId = estadoAnteriorId,
            EsFinal = esEstadoFinal,
            SolicitudId = solicitudId
        }, transaction);

        if (filasAfectadas == 0)
            throw new SmartTicket.Application.Common.Exceptions.ReglaNegocioException(
                "La solicitud cambió de estado mientras la actualizabas. Recarga e inténtalo de nuevo.");

        const string historialSql = @"
            INSERT INTO dbo.SolicitudHistorialEstados (SolicitudId, EstadoAnteriorId, EstadoNuevoId, UsuarioId, FechaCambio)
            VALUES (@SolicitudId, @EstadoAnteriorId, @EstadoNuevoId, @UsuarioId, @FechaCambio)";

        await connection.ExecuteAsync(historialSql, new
        {
            SolicitudId = solicitudId,
            EstadoAnteriorId = estadoAnteriorId,
            EstadoNuevoId = estadoNuevoId,
            UsuarioId = usuarioId,
            FechaCambio = DateTime.UtcNow
        }, transaction);

        transaction.Commit();
    }

    public async Task AsignarAsync(long solicitudId, int usuarioAsignadoId)
    {
        const string sql = @"UPDATE Solicitudes SET UsuarioAsignadoId = @UsuarioAsignadoId WHERE SolicitudId = @SolicitudId";

        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(sql, new { UsuarioAsignadoId = usuarioAsignadoId, SolicitudId = solicitudId });
    }

    public async Task<IEnumerable<Tbl_Solicitud>> GetByClienteIdAsync(int clienteId)
    {
        const string sql = @"
            SELECT SolicitudId, ClienteId, CategoriaId, SubcategoriaId, PrioridadId, AreaId, EstadoId,
                   UsuarioAsignadoId, TextoOriginal, Puntuacion, FechaCreacion, FechaLimiteSLA, FechaCierre
            FROM dbo.Solicitudes
            WHERE ClienteId = @ClienteId
            ORDER BY FechaCreacion DESC";

        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<Tbl_Solicitud>(sql, new { ClienteId = clienteId });
    }
}
