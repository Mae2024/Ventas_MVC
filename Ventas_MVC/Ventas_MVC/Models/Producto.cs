namespace Ventas_MVC.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string? Categoria { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}
