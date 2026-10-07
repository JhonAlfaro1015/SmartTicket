using Dapper;
using SmartTicket.Application.Interface;
using SmartTicket.Domain.Entity;
using SmartTicket.Infrastructure.Persistence;

namespace SmartTicket.Infrastructure.Repositories;

public class UsuarioRepository(DapperContext context) : IUsuarioRepository
{
    private readonly DapperContext _context = context;

    public async Task<Tbl_Usuario?> GetByEmailAsync(string email)
    {
        const string sql = @"SELECT UsuarioId, RolId, Nombre, Email, PasswordHash, Activo, FechaCreacion FROM Usuarios WHERE Email = @Email";

        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Tbl_Usuario>(sql, new { Email = email });
    }

    public async Task<Tbl_Usuario?> GetByIdAsync(int usuarioId)
    {
        const string sql = @"SELECT UsuarioId, RolId, Nombre, Email, PasswordHash, Activo, FechaCreacion FROM Usuarios WHERE UsuarioId = @UsuarioId";

        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Tbl_Usuario>(sql, new { UsuarioId = usuarioId });
    }

    public async Task<int> CreateAsync(Tbl_Usuario usuario)
    {
        const string sql = @"INSERT INTO Usuarios (RolId, Nombre, Email, PasswordHash, Activo, FechaCreacion) VALUES (@RolId, @Nombre, @Email, @PasswordHash, @Activo, @FechaCreacion); SELECT CAST(SCOPE_IDENTITY() AS INT);";

        using var connection = _context.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(sql, usuario);
    }

    public async Task<IEnumerable<Tbl_Usuario>> GetByRolNombreAsync(string rolNombre)
    {
        const string sql = @"
            SELECT u.UsuarioId, u.RolId, u.Nombre, u.Email, u.PasswordHash, u.Activo, u.FechaCreacion
            FROM dbo.Usuarios u
            JOIN dbo.Roles r ON r.RolId = u.RolId
            WHERE r.Nombre = @RolNombre AND u.Activo = 1
            ORDER BY u.Nombre";

        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<Tbl_Usuario>(sql, new { RolNombre = rolNombre });
    }
}