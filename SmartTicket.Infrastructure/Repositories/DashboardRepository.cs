using Dapper;
using SmartTicket.Application.Common;
using SmartTicket.Application.Dtos.Dashboard;
using SmartTicket.Application.Interface;
using SmartTicket.Infrastructure.Persistence;

namespace SmartTicket.Infrastructure.Repositories;

internal class ResolucionesFila
{
    public int Resueltas { get; set; }
    public double? PromedioHoras { get; set; }
    public int ConSla { get; set; }
    public int EnSla { get; set; }
}

internal class ContadoresFila
{
    public int Pendientes { get; set; }
    public int SinAsignar { get; set; }
    public int Vencidas { get; set; }
    public int EnRiesgo { get; set; }
    public int Creadas7 { get; set; }
    public int Previas7 { get; set; }
}

public class DashboardRepository : IDashboardRepository
{
    private readonly DapperContext _context;

    public DashboardRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<DashboardResumenDto> GetResumenAsync()
    {
        const string sql = @"
            -- 1) total
            SELECT COUNT(*) FROM dbo.Solicitudes;
 
            -- 2) por estado (incluye los que tienen 0, en el orden del flujo)
            SELECT e.Nombre, COUNT(s.SolicitudId) AS Cantidad
            FROM dbo.Estados e
            LEFT JOIN dbo.Solicitudes s ON s.EstadoId = e.EstadoId
            GROUP BY e.EstadoId, e.Nombre, e.Orden
            ORDER BY e.Orden, e.EstadoId;
 
            -- 3) por prioridad (de la más a la menos urgente)
            SELECT p.Nombre, COUNT(s.SolicitudId) AS Cantidad
            FROM dbo.Prioridades p
            LEFT JOIN dbo.Solicitudes s ON s.PrioridadId = p.PrioridadId
            GROUP BY p.PrioridadId, p.Nombre, p.NivelUrgencia
            ORDER BY p.NivelUrgencia;
 
            -- 4) por categoría
            SELECT c.Nombre, COUNT(s.SolicitudId) AS Cantidad
            FROM dbo.Categorias c
            LEFT JOIN dbo.Solicitudes s ON s.CategoriaId = c.CategoriaId
            GROUP BY c.CategoriaId, c.Nombre
            ORDER BY COUNT(s.SolicitudId) DESC, c.Nombre;
 
            -- 5) tiempo promedio hasta el cierre (métrica anterior, se conserva)
            SELECT AVG(CAST(DATEDIFF(MINUTE, FechaCreacion, FechaCierre) AS FLOAT)) / 60.0
            FROM dbo.Solicitudes
            WHERE FechaCierre IS NOT NULL;
 
            -- 6) resolución real: desde la creación hasta el PRIMER paso a 'Resuelto'
            ;WITH Resueltas AS (
                SELECT s.SolicitudId, s.FechaCreacion, s.FechaLimiteSLA, MIN(h.FechaCambio) AS FechaResolucion
                FROM dbo.Solicitudes s
                JOIN dbo.SolicitudHistorialEstados h ON h.SolicitudId = s.SolicitudId
                JOIN dbo.Estados e ON e.EstadoId = h.EstadoNuevoId AND e.Nombre = @Resuelto
                GROUP BY s.SolicitudId, s.FechaCreacion, s.FechaLimiteSLA
            )
            SELECT COUNT(*) AS Resueltas,
                   AVG(CAST(DATEDIFF(MINUTE, FechaCreacion, FechaResolucion) AS FLOAT)) / 60.0 AS PromedioHoras,
                   ISNULL(SUM(CASE WHEN FechaLimiteSLA IS NOT NULL THEN 1 ELSE 0 END), 0) AS ConSla,
                   ISNULL(SUM(CASE WHEN FechaLimiteSLA IS NOT NULL AND FechaResolucion <= FechaLimiteSLA THEN 1 ELSE 0 END), 0) AS EnSla
            FROM Resueltas;
 
            -- 7) contadores de trabajo abierto, SLA y volumen reciente
            SELECT
                ISNULL(SUM(CASE WHEN e.Nombre IN (@Nuevo, @EnProceso) THEN 1 ELSE 0 END), 0) AS Pendientes,
                ISNULL(SUM(CASE WHEN e.Nombre IN (@Nuevo, @EnProceso) AND s.UsuarioAsignadoId IS NULL THEN 1 ELSE 0 END), 0) AS SinAsignar,
                ISNULL(SUM(CASE WHEN e.Nombre IN (@Nuevo, @EnProceso) AND s.FechaLimiteSLA < SYSUTCDATETIME() THEN 1 ELSE 0 END), 0) AS Vencidas,
                ISNULL(SUM(CASE WHEN e.Nombre IN (@Nuevo, @EnProceso)
                                 AND s.FechaLimiteSLA >= SYSUTCDATETIME()
                                 AND s.FechaLimiteSLA <= DATEADD(HOUR, @HorasEnRiesgo, SYSUTCDATETIME()) THEN 1 ELSE 0 END), 0) AS EnRiesgo,
                ISNULL(SUM(CASE WHEN s.FechaCreacion >= DATEADD(DAY, -7, SYSUTCDATETIME()) THEN 1 ELSE 0 END), 0) AS Creadas7,
                ISNULL(SUM(CASE WHEN s.FechaCreacion >= DATEADD(DAY, -14, SYSUTCDATETIME())
                                 AND s.FechaCreacion <  DATEADD(DAY, -7, SYSUTCDATETIME()) THEN 1 ELSE 0 END), 0) AS Previas7
            FROM dbo.Solicitudes s
            JOIN dbo.Estados e ON e.EstadoId = s.EstadoId;
 
            -- 8) carga por agente (incluye agentes sin solicitudes)
            SELECT u.UsuarioId, u.Nombre,
                   ISNULL(SUM(CASE WHEN e.Nombre IN (@Nuevo, @EnProceso) THEN 1 ELSE 0 END), 0) AS Pendientes,
                   ISNULL(SUM(CASE WHEN e.Nombre IN (@Resuelto, @Cerrado) THEN 1 ELSE 0 END), 0) AS Resueltas,
                   COUNT(s.SolicitudId) AS Total
            FROM dbo.Usuarios u
            JOIN dbo.Roles r ON r.RolId = u.RolId AND r.Nombre = @Agente
            LEFT JOIN dbo.Solicitudes s ON s.UsuarioAsignadoId = u.UsuarioId
            LEFT JOIN dbo.Estados e ON e.EstadoId = s.EstadoId
            WHERE u.Activo = 1
            GROUP BY u.UsuarioId, u.Nombre
            ORDER BY Pendientes DESC, u.Nombre;
 
            -- 9) tendencia diaria (días en UTC)
            ;WITH Dias AS (
                SELECT DATEADD(DAY, -(@DiasTendencia - 1), CAST(SYSUTCDATETIME() AS DATE)) AS Dia
                UNION ALL
                SELECT DATEADD(DAY, 1, Dia) FROM Dias WHERE Dia < CAST(SYSUTCDATETIME() AS DATE)
            )
            SELECT d.Dia AS Fecha,
                   (SELECT COUNT(*) FROM dbo.Solicitudes s
                     WHERE CAST(s.FechaCreacion AS DATE) = d.Dia) AS Creadas,
                   (SELECT COUNT(*) FROM dbo.Solicitudes s
                     WHERE s.FechaCierre IS NOT NULL AND CAST(s.FechaCierre AS DATE) = d.Dia) AS Finalizadas
            FROM Dias d
            ORDER BY d.Dia
            OPTION (MAXRECURSION 400);";

        var parametros = new
        {
            Nuevo = EstadosSistema.Nuevo,
            EnProceso = EstadosSistema.EnProceso,
            Resuelto = EstadosSistema.Resuelto,
            Cerrado = EstadosSistema.Cerrado,
            Agente = RolesSistema.Agente,
            HorasEnRiesgo = DashboardReglas.HorasEnRiesgo,
            DiasTendencia = DashboardReglas.DiasTendencia
        };

        using var connection = _context.CreateConnection();
        using var multi = await connection.QueryMultipleAsync(sql, parametros);

        var total = await multi.ReadFirstAsync<int>();
        var porEstado = (await multi.ReadAsync<ConteoDto>()).ToList();
        var porPrioridad = (await multi.ReadAsync<ConteoDto>()).ToList();
        var porCategoria = (await multi.ReadAsync<ConteoDto>()).ToList();
        var promedioAtencion = await multi.ReadFirstOrDefaultAsync<double?>();
        var resoluciones = await multi.ReadFirstAsync<ResolucionesFila>();
        var contadores = await multi.ReadFirstAsync<ContadoresFila>();
        var porAgente = (await multi.ReadAsync<AgenteCargaDto>()).ToList();
        var tendencia = (await multi.ReadAsync<PuntoTendenciaDto>()).ToList();

        return new DashboardResumenDto
        {
            TotalSolicitudes = total,
            PorEstado = porEstado,
            PorPrioridad = porPrioridad,
            PorCategoria = porCategoria,
            TiempoPromedioAtencionHoras = promedioAtencion,

            Pendientes = contadores.Pendientes,
            SinAsignar = contadores.SinAsignar,
            Vencidas = contadores.Vencidas,
            EnRiesgo = contadores.EnRiesgo,
            HorasEnRiesgo = DashboardReglas.HorasEnRiesgo,
            CreadasUltimos7Dias = contadores.Creadas7,
            CreadasPrevios7Dias = contadores.Previas7,

            Resueltas = resoluciones.Resueltas,
            TiempoPromedioResolucionHoras = resoluciones.PromedioHoras,
            CumplimientoSlaPorcentaje = resoluciones.ConSla > 0
                ? Math.Round(resoluciones.EnSla * 100.0 / resoluciones.ConSla, 1)
                : null,

            PorAgente = porAgente,
            Tendencia = tendencia
        };
    }
}
