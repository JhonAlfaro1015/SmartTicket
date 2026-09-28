namespace SmartTicket.Application.Dtos.Dashboard;

public class DashboardResumenDto
{
    public int TotalSolicitudes { get; set; }
    public List<ConteoDto> PorEstado { get; set; } = [];
    public List<ConteoDto> PorPrioridad { get; set; } = [];
    public List<ConteoDto> PorCategoria { get; set; } = [];
    public double? TiempoPromedioAtencionHoras { get; set; }
}
