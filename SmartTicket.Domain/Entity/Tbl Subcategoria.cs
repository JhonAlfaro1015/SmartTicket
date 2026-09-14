namespace SmartTicket.Domain.Entity
{
    public class Tbl_Subcategoria
    {
        public int SubcategoriaId { get; set; }
        public int CategoriaId { get; set; }
        public string Nombre { get; set; } = null!;
        public bool Activo { get; set; }
    }
}
