using Dapper;
using SmartTicket.Application.Interface;
using SmartTicket.Domain.Entity;
using SmartTicket.Infrastructure.Persistence;

namespace SmartTicket.Infrastructure.Repositories;

public class RolRepository(DapperContext context) : IRolRepository
{
    private readonly DapperContext _context = context;

    public async Task<Tbl_Rol?> GetByIdAsync(int rolId)
    {
        const string sql = "SELECT RolId, Nombre FROM Roles WHERE RolId = @RolId";

        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Tbl_Rol>(sql, new { RolId = rolId });
    }

    public async Task<Tbl_Rol?> GetByNombreAsync(string nombre)
    {
        const string sql = "SELECT RolId, Nombre FROM Roles WHERE Nombre = @Nombre";

        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Tbl_Rol>(sql, new { Nombre = nombre });
    }
}