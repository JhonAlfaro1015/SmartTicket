namespace SmartTicket.Domain.Entity
{
    public class Tbl_Estado
    {
        public int EstadoId { get; set; }
        public string Nombre { get; set; } = null!;
        public bool EsFinal { get; set; }
    }
}
