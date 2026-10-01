namespace Ventas_MVC.Models
{
    public class Venta
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public int ProductoId { get; set; }
        public DateTime Fecha { get; set; }
        public Cliente Cliente { get; set; }
        public Producto Producto { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Monto { get; set; }
    }
}
