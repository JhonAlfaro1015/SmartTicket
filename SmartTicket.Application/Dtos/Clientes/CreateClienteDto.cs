namespace SmartTicket.Application.Dtos.Clientes;

public class CreateClienteDto
{
    public string Nombre { get; set; } = null!;
    public string? Email { get; set; }
    public string? Telefono { get; set; }
}
