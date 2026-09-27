namespace SmartTicket.Application.Dtos.Solicitudes;

public class SolicitudResponseDto
{
    public long SolicitudId { get; set; }
    public int ClienteId { get; set; }
    public int CategoriaId { get; set; }
    public int? SubcategoriaId { get; set; }
    public int PrioridadId { get; set; }
    public int AreaId { get; set; }
    public int EstadoId { get; set; }
    public int? UsuarioAsignadoId { get; set; }
    public string TextoOriginal { get; set; } = null!;
    public decimal? Puntuacion { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaLimiteSLA { get; set; }
    public DateTime? FechaCierre { get; set; }
}
