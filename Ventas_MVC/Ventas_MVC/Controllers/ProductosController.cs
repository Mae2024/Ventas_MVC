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
            var productos = _contexto.Productos.OrderBy(p => p.Nombre).ToList();
            return View(productos);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var producto = _contexto.Productos.Find(id);
            if (producto == null) return NotFound();
            return View(producto);

        }

        [HttpPost]
        public IActionResult Edit(int id, Producto producto)
        {
            var original = _contexto.Productos.Find(id);
            if (original == null) return NotFound();

            original.Nombre = producto.Nombre;
            original.PrecioUnitario = producto.PrecioUnitario;
            _contexto.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var producto = _contexto.Productos.Find(id);
            if (producto == null) return NotFound();

            ViewBag.CantidadVentas = _contexto.Ventas.Count(v => v.ProductoId == id);
            return View(producto);
        }


        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            if (_contexto.Ventas.Any(v => v.ProductoId == id))
                return RedirectToAction("Delete", new { id = id });

            var producto = _contexto.Productos.Find(id);
            if (producto != null)
            {
                _contexto.Productos.Remove(producto);
                _contexto.SaveChanges();
            }
            return RedirectToAction("Index");

        }
    }
}
