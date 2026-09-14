namespace SmartTicket.Domain.Entity
{
    public class Tbl_Prioridad
    {
        public int PrioridadId { get; set; }
        public string Nombre { get; set; } = null!;
        public byte NivelUrgencia { get; set; } // 1 = más urgente
    }
}
