namespace SmartTicket.Domain.Entity
{
    public class Tbl_SolicitudHistorialEstado
    {
        public long HistorialId { get; set; }
        public long SolicitudId { get; set; }
        public int? EstadoAnteriorId { get; set; }
        public int EstadoNuevoId { get; set; }
        public int UsuarioId { get; set; }
        public DateTime FechaCambio { get; set; }
    }
}
