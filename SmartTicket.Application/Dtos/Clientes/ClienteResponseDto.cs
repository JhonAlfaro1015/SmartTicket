namespace SmartTicket.Application.Dtos.Clientes;

public class ClienteResponseDto
{
    public int ClienteId { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Email { get; set; }
    public string? Telefono { get; set; }
    public DateTime FechaCreacion { get; set; }
}
