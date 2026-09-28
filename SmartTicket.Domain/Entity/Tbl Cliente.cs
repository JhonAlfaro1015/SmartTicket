namespace SmartTicket.Domain.Entity
{
    public class Tbl_Cliente
    {
        public int ClienteId { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public string? PasswordHash { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
