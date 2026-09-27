using Dapper;
using SmartTicket.Application.Interface;
using SmartTicket.Domain.Entity;
using SmartTicket.Infrastructure.Persistence;

namespace SmartTicket.Infrastructure.Repositories;

public class ClienteRepository(DapperContext context) : IClienteRepository
{
    private readonly DapperContext _context = context;

    public async Task<int> CreateAsync(Tbl_Cliente cliente)
    {
        const string sql = @"INSERT INTO Clientes (Nombre, Email, Telefono, FechaCreacion) VALUES (@Nombre, @Email, @Telefono, @FechaCreacion); SELECT CAST(SCOPE_IDENTITY() AS INT);";

        using var connection = _context.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(sql, cliente);
    }

    public async Task<Tbl_Cliente?> GetByIdAsync(int clienteId)
    {
        const string sql = @"SELECT ClienteId, Nombre, Email, Telefono, FechaCreacion FROM Clientes WHERE ClienteId = @ClienteId";

        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Tbl_Cliente>(sql, new { ClienteId = clienteId });
    }
}
