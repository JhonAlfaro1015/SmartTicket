using Dapper;
using SmartTicket.Application.Interface;
using SmartTicket.Domain.Entity;
using SmartTicket.Infrastructure.Persistence;

namespace SmartTicket.Infrastructure.Repositories;

public class EstadoRepository(DapperContext context) : IEstadoRepository
{
    private readonly DapperContext _context = context;

    public async Task<Tbl_Estado?> GetByIdAsync(int estadoId)
    {
        const string sql = "SELECT EstadoId, Nombre, EsFinal FROM Estados WHERE EstadoId = @EstadoId";
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Tbl_Estado>(sql, new { EstadoId = estadoId });
    }

    public async Task<Tbl_Estado?> GetByNombreAsync(string nombre)
    {
        const string sql = "SELECT EstadoId, Nombre, EsFinal FROM dbo.Estados WHERE Nombre = @Nombre";
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Tbl_Estado>(sql, new { Nombre = nombre });
    }
}
