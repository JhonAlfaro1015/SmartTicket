namespace SmartTicket.Application.Dtos.Auth;

public class RegisterRequestDto
{
    public string Nombre { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string RolNombre { get; set; } = null!;
}
