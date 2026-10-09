using Microsoft.AspNetCore.Mvc;
using Ventas_MVC.Data;
using Ventas_MVC.Models;

namespace Ventas_MVC.Controllers
{
    public class ClientesController : Controller
    {
        private readonly VentasContext _contexto;
        public ClientesController(VentasContext contexto)
        {
            _contexto = contexto;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Cliente cliente)
        {
            _contexto.Clientes.Add(cliente);
            _contexto.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Index()
        {
            var clientes = _contexto.Clientes.OrderBy(c => c.Nombre).ToList();
            return View(clientes);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var cliente = _contexto.Clientes.Find(id);
            if (cliente == null) return NotFound();
            return View(cliente);

        }

        [HttpPost]
        public IActionResult Edit(int id, Cliente cliente)
        {
            var original = _contexto.Clientes.Find(id);
            if (original == null) return NotFound();

            original.Nombre = cliente.Nombre;
            original.Direccion = cliente.Direccion;
            _contexto.SaveChanges();

            return RedirectToAction("Index");

        }

        [HttpGet]
        public IActionResult Delete(int id) 
        {
            var cliente = _contexto.Clientes.Find(id);
            if (cliente == null) return NotFound();

            ViewBag.CantidadVentas = _contexto.Ventas.Count(v => v.ClienteId == id);

            return View(cliente);
        }


        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            if (_contexto.Ventas.Any(v => v.ClienteId == id))
            {
                return RedirectToAction("Delete", new { id });
            }

            var cliente = _contexto.Clientes.Find(id);
            if (cliente != null)
            {
                _contexto.Clientes.Remove(cliente);
                _contexto.SaveChanges();
            }

            return RedirectToAction("Index");
        }


    }
}
