using Microsoft.EntityFrameworkCore;
using Ventas_MVC.Models;

namespace Ventas_MVC.Data
{
    public class VentasContext : DbContext
    {
        public VentasContext(DbContextOptions<VentasContext> options) : base(options)
        {
        }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Producto> Productos { get; set; }

    }
}
