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
            return RedirectToAction("Create");
        }

    }
}
