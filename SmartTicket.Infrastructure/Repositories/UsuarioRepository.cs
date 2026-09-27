using Dapper;
using SmartTicket.Application.Interface;
using SmartTicket.Domain.Entity;
using SmartTicket.Infrastructure.Persistence;

namespace SmartTicket.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly DapperContext _context;

    public UsuarioRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<Tbl_Usuario?> GetByEmailAsync(string email)
    {
        const string sql = @"SELECT UsuarioId, RolId, Nombre, Email, PasswordHash, Activo, FechaCreacion FROM Usuarios WHERE Email = @Email";

        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Tbl_Usuario>(sql, new { Email = email });
    }

    public async Task<int> CreateAsync(Tbl_Usuario usuario)
    {
        const string sql = @"INSERT INTO Usuarios (RolId, Nombre, Email, PasswordHash, Activo, FechaCreacion) VALUES (@RolId, @Nombre, @Email, @PasswordHash, @Activo, @FechaCreacion); SELECT CAST(SCOPE_IDENTITY() AS INT);";

        using var connection = _context.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(sql, usuario);
    }
}