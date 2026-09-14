namespace SmartTicket.Domain.Entity
{
    public class Tbl_Categoria
    {
        public int CategoriaId { get; set; }
        public string Nombre { get; set; } = null!;
        public bool Activo { get; set; }
    }
}
