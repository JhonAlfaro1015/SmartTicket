namespace SmartTicket.Domain.Entity
{
    public class Tbl_SolicitudObservacion
    {
        public long ObservacionId { get; set; }
        public long SolicitudId { get; set; }
        public int UsuarioId { get; set; }
        public string Texto { get; set; } = null!;
        public DateTime FechaCreacion { get; set; }
    }
}
