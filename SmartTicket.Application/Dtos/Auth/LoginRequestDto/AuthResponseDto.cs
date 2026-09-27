namespace SmartTicket.Application.Dtos.Auth.LoginRequestDto
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Rol { get; set; } = null!;
        public DateTime ExpiraEn { get; set; }
    }
}
