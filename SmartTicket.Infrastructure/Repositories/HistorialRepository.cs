using Dapper;
using SmartTicket.Application.Dtos.Solicitudes;
using SmartTicket.Application.Interface;
using SmartTicket.Infrastructure.Persistence;

namespace SmartTicket.Infrastructure.Repositories;

public class HistorialRepository : IHistorialRepository
{
    private readonly DapperContext _context;

    public HistorialRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<HistorialEstadoDto>> ListarPorSolicitudAsync(long solicitudId)
    {
        const string sql = @"
            SELECT h.HistorialId, h.SolicitudId,
                   ea.Nombre AS EstadoAnterior, en.Nombre AS EstadoNuevo,
                   u.Nombre AS UsuarioNombre, r.Nombre AS UsuarioRol,
                   h.FechaCambio
            FROM dbo.SolicitudHistorialEstados h
            LEFT JOIN dbo.Estados ea ON ea.EstadoId = h.EstadoAnteriorId
            JOIN dbo.Estados en      ON en.EstadoId = h.EstadoNuevoId
            JOIN dbo.Usuarios u      ON u.UsuarioId = h.UsuarioId
            JOIN dbo.Roles r         ON r.RolId = u.RolId
            WHERE h.SolicitudId = @SolicitudId
            ORDER BY h.FechaCambio, h.HistorialId";

        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<HistorialEstadoDto>(sql, new { SolicitudId = solicitudId });
    }
}
