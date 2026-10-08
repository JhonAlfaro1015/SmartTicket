using Dapper;
using SmartTicket.Application.Dtos.Comentarios;
using SmartTicket.Application.Interface;
using SmartTicket.Infrastructure.Persistence;

namespace SmartTicket.Infrastructure.Repositories;

public class ComentarioRepository(DapperContext context) : IComentarioRepository
{
    private readonly DapperContext _context = context;
    private const string SelectBase = @"
        SELECT o.ObservacionId AS ComentarioId,
               o.SolicitudId,
               o.Texto,
               o.FechaCreacion,
               CASE WHEN o.ClienteId IS NOT NULL THEN c.Nombre ELSE u.Nombre END AS AutorNombre,
               CASE WHEN o.ClienteId IS NOT NULL THEN N'Cliente' ELSE r.Nombre END AS AutorRol,
               CAST(CASE WHEN o.ClienteId IS NOT NULL THEN 1 ELSE 0 END AS BIT) AS EsCliente
        FROM dbo.SolicitudObservaciones o
        LEFT JOIN dbo.Clientes c ON c.ClienteId = o.ClienteId
        LEFT JOIN dbo.Usuarios u ON u.UsuarioId = o.UsuarioId
        LEFT JOIN dbo.Roles r ON r.RolId = u.RolId";

    public async Task<long> AgregarAsync(long solicitudId, int? usuarioId, int? clienteId, string texto)
    {
        const string sql = @"
            INSERT INTO dbo.SolicitudObservaciones (SolicitudId, UsuarioId, ClienteId, Texto)
            VALUES (@SolicitudId, @UsuarioId, @ClienteId, @Texto);
            SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

        using var connection = _context.CreateConnection();
        return await connection.ExecuteScalarAsync<long>(sql, new
        {
            SolicitudId = solicitudId,
            UsuarioId = usuarioId,
            ClienteId = clienteId,
            Texto = texto
        });
    }

    public async Task<ComentarioDto?> ObtenerPorIdAsync(long comentarioId)
    {
        var sql = SelectBase + " WHERE o.ObservacionId = @ComentarioId";

        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<ComentarioDto>(sql, new { ComentarioId = comentarioId });
    }

    public async Task<IEnumerable<ComentarioDto>> ListarPorSolicitudAsync(long solicitudId)
    {
        // Orden cronológico; el Id desempata comentarios del mismo segundo
        var sql = SelectBase + " WHERE o.SolicitudId = @SolicitudId ORDER BY o.FechaCreacion, o.ObservacionId";

        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<ComentarioDto>(sql, new { SolicitudId = solicitudId });
    }
}
