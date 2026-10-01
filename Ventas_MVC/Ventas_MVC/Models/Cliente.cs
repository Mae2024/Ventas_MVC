namespace Ventas_MVC.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public ICollection<Venta> Ventas { get; set; }
    }
}
