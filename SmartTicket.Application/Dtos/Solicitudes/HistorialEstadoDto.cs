namespace SmartTicket.Application.Dtos.Solicitudes;

public class HistorialEstadoDto
{
    public long HistorialId { get; set; }
    public long SolicitudId { get; set; }
    public string? EstadoAnterior { get; set; }
    public string EstadoNuevo { get; set; } = null!;
    public string UsuarioNombre { get; set; } = null!;
    public string UsuarioRol { get; set; } = null!;
    public DateTime FechaCambio { get; set; }
}
