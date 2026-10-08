namespace SmartTicket.Application.Dtos.Comentarios;

public class ComentarioDto
{
    public long ComentarioId { get; set; }
    public long SolicitudId { get; set; }
    public string Texto { get; set; } = null!;
    public DateTime FechaCreacion { get; set; }
    public string AutorNombre { get; set; } = null!;
    public string AutorRol { get; set; } = null!;   // "Cliente", "Agente", "Supervisor", "Administrador"
    public bool EsCliente { get; set; }             // el frontend lo usa para diferenciar burbujas
}
