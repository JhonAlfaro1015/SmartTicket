using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace SmartTicket.Infrastructure.Persistence;

public class DapperContext(IConfiguration configuration)
{
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'DefaultConnection'.");

    public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
}