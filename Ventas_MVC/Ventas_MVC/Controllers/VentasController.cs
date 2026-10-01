using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ventas_MVC.Data;

namespace Ventas_MVC.Controllers
{
    public class VentasController : Controller
    {
        private VentasContext _contexto;

        public VentasController(VentasContext contexto)
        {
            _contexto = contexto;
        }

        public IActionResult Index()
        {
            var ventas = _contexto.Ventas
                .Include(v=>v.Cliente)
                .Include(v=>v.Producto)
                .ToList();

            return View(ventas);
        }
    }
}
