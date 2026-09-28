using Dapper;
using SmartTicket.Application.Dtos.Dashboard;
using SmartTicket.Application.Interface;
using SmartTicket.Infrastructure.Persistence;

namespace SmartTicket.Infrastructure.Repositories;

public class DashboardRepository(DapperContext context) : IDashboardRepository
{
    private readonly DapperContext _context = context;

    public async Task<DashboardResumenDto> GetResumenAsync()
    {
        const string sql = @"SELECT COUNT(*) AS Cantidad FROM Solicitudes;
 
            SELECT e.Nombre, COUNT(*) AS Cantidad FROM Solicitudes s JOIN Estados e ON e.EstadoId = s.EstadoId GROUP BY e.Nombre;
 
            SELECT p.Nombre, COUNT(*) AS Cantidad FROM Solicitudes s JOIN Prioridades p ON p.PrioridadId = s.PrioridadId
            GROUP BY p.Nombre;
 
            SELECT c.Nombre, COUNT(*) AS Cantidad FROM Solicitudes s JOIN Categorias c ON c.CategoriaId = s.CategoriaId GROUP BY c.Nombre;
 
            SELECT AVG(CAST(DATEDIFF(HOUR, FechaCreacion, FechaCierre) AS FLOAT)) AS Promedio FROM Solicitudes WHERE FechaCierre IS NOT NULL;";

        using var connection = _context.CreateConnection();
        using var multi = await connection.QueryMultipleAsync(sql);

        var total = await multi.ReadFirstAsync<int>();
        var porEstado = (await multi.ReadAsync<ConteoDto>()).ToList();
        var porPrioridad = (await multi.ReadAsync<ConteoDto>()).ToList();
        var porCategoria = (await multi.ReadAsync<ConteoDto>()).ToList();
        var promedioHoras = await multi.ReadFirstOrDefaultAsync<double?>();

        return new DashboardResumenDto
        {
            TotalSolicitudes = total,
            PorEstado = porEstado,
            PorPrioridad = porPrioridad,
            PorCategoria = porCategoria,
            TiempoPromedioAtencionHoras = promedioHoras
        };
    }
}
