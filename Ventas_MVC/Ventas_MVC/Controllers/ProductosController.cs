using Microsoft.AspNetCore.Mvc;
using Ventas_MVC.Data;
using Ventas_MVC.Models;

namespace Ventas_MVC.Controllers
{
    public class ProductosController : Controller
    {
        private readonly VentasContext _contexto;
        public ProductosController(VentasContext contexto)
        {
            _contexto = contexto;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Producto producto)
        {
            _contexto.Productos.Add(producto);
            _contexto.SaveChanges();
            return RedirectToAction("Create");
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
