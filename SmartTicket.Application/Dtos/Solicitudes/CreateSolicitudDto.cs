namespace SmartTicket.Application.Dtos.Solicitudes;

public class CreateSolicitudDto
{
    public int ClienteId { get; set; }
    public string TextoOriginal { get; set; } = null!;
}
