using System.ComponentModel.DataAnnotations;

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
        [Required(ErrorMessage = "La cantidad es requerida")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "La cantidad debe ser un número decimal con hasta 2 decimales")]
        public decimal Cantidad { get; set; }
        [Required(ErrorMessage = "La cantidad es requerida")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "La cantidad debe ser un número decimal con hasta 2 decimales")]
        public decimal Monto { get; set; }
    }
}
