using Dapper;
using SmartTicket.Application.Dtos.Solicitudes;
using SmartTicket.Application.Interface;
using SmartTicket.Infrastructure.Persistence;

namespace SmartTicket.Infrastructure.Repositories;

public class SolicitudLecturaRepository(DapperContext context) : ISolicitudLecturaRepository
{
    private readonly DapperContext _context = context;
    private const string SelectBase = @"
        SELECT s.SolicitudId,
               s.ClienteId,        cl.Nombre AS ClienteNombre,
               s.CategoriaId,      ca.Nombre AS CategoriaNombre,
               s.SubcategoriaId,   sc.Nombre AS SubcategoriaNombre,
               s.PrioridadId,      p.Nombre  AS PrioridadNombre, p.NivelUrgencia AS PrioridadNivel,
               s.AreaId,           a.Nombre  AS AreaNombre,
               s.EstadoId,         e.Nombre  AS EstadoNombre,    e.EsFinal AS EstadoEsFinal,
               s.UsuarioAsignadoId, u.Nombre AS AgenteNombre,
               s.TextoOriginal, s.Puntuacion, s.FechaCreacion, s.FechaLimiteSLA, s.FechaCierre
        FROM dbo.Solicitudes s
        JOIN dbo.Clientes cl        ON cl.ClienteId = s.ClienteId
        JOIN dbo.Categorias ca      ON ca.CategoriaId = s.CategoriaId
        LEFT JOIN dbo.Subcategorias sc ON sc.SubcategoriaId = s.SubcategoriaId
        JOIN dbo.Prioridades p      ON p.PrioridadId = s.PrioridadId
        JOIN dbo.Areas a            ON a.AreaId = s.AreaId
        JOIN dbo.Estados e          ON e.EstadoId = s.EstadoId
        LEFT JOIN dbo.Usuarios u    ON u.UsuarioId = s.UsuarioAsignadoId";

    public async Task<IEnumerable<SolicitudDetalleDto>> ListarAsync(
        int? estadoId, int? prioridadId, int? clienteId, int? usuarioAsignadoId)
    {
        var sql = SelectBase + " WHERE 1 = 1";
        if (estadoId.HasValue) sql += " AND s.EstadoId = @EstadoId";
        if (prioridadId.HasValue) sql += " AND s.PrioridadId = @PrioridadId";
        if (clienteId.HasValue) sql += " AND s.ClienteId = @ClienteId";
        if (usuarioAsignadoId.HasValue) sql += " AND s.UsuarioAsignadoId = @UsuarioAsignadoId";
        sql += " ORDER BY s.FechaCreacion DESC";

        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<SolicitudDetalleDto>(sql, new
        {
            EstadoId = estadoId,
            PrioridadId = prioridadId,
            ClienteId = clienteId,
            UsuarioAsignadoId = usuarioAsignadoId
        });
    }

    public async Task<SolicitudDetalleDto?> ObtenerAsync(long solicitudId)
    {
        var sql = SelectBase + " WHERE s.SolicitudId = @SolicitudId";

        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<SolicitudDetalleDto>(sql, new { SolicitudId = solicitudId });
    }
}
