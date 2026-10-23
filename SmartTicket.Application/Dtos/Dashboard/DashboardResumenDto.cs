using SmartTicket.Application.Dtos.Solicitudes;

namespace SmartTicket.Application.Dtos.Dashboard;

public class AgenteCargaDto
{
    public int UsuarioId { get; set; }
    public string Nombre { get; set; } = null!;
    public int Pendientes { get; set; }
    public int Resueltas { get; set; }
    public int Total { get; set; }
}

public class PuntoTendenciaDto
{
    public DateTime Fecha { get; set; }
    public int Creadas { get; set; }
    public int Finalizadas { get; set; }
}

public class DashboardResumenDto
{
    // --- campos que ya existían ---
    public int TotalSolicitudes { get; set; }
    public List<ConteoDto> PorEstado { get; set; } = new();
    public List<ConteoDto> PorPrioridad { get; set; } = new();
    public List<ConteoDto> PorCategoria { get; set; } = new();
    public double? TiempoPromedioAtencionHoras { get; set; }

    // --- nuevos ---
    public int Pendientes { get; set; }
    public int SinAsignar { get; set; }
    public int Vencidas { get; set; }
    public int EnRiesgo { get; set; }
    public int HorasEnRiesgo { get; set; }
    public int CreadasUltimos7Dias { get; set; }
    public int CreadasPrevios7Dias { get; set; }
    public int Resueltas { get; set; }
    public double? TiempoPromedioResolucionHoras { get; set; }
    public double? CumplimientoSlaPorcentaje { get; set; }
    public List<AgenteCargaDto> PorAgente { get; set; } = new();
    public List<PuntoTendenciaDto> Tendencia { get; set; } = new();
    public List<SolicitudDetalleDto> Urgentes { get; set; } = new();
}
