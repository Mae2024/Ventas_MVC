using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ventas_MVC.Data;
using Ventas_MVC.Models;
using System;


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
                .Include(v => v.Cliente)
                .Include(v => v.Producto)
                .ToList();

            return View(ventas);
        }



        [HttpGet]
        public IActionResult Create(int? ClienteId)
        {
            ViewBag.Clientes = new SelectList(_contexto.Clientes, "Id", "Nombre");
            ViewBag.Productos = new SelectList(_contexto.Productos, "Id", "Nombre");

            if(ClienteId != null)
            {
                var hoy = DateTime.Today;
                ViewBag.VentasDelDia = _contexto.Ventas
                    .Include(v=>v.Producto)
                    .Where(v => v.ClienteId == ClienteId && v.Fecha.Date == hoy)
                    .ToList();

            }


            return View(new Venta {ClienteId = ClienteId ?? 0 });
        }



        [HttpPost]
        public IActionResult Create(Venta venta)
        {
            var producto = _contexto.Productos.Find(venta.ProductoId);
            if (producto == null)
            {
                return RedirectToAction("Create", new { ClienteId = venta.ClienteId } );

            }

            venta.Monto = venta.Cantidad * producto.PrecioUnitario;
            venta.Fecha = DateTime.Now;

            _contexto.Ventas.Add(venta);
            _contexto.SaveChanges();

            return RedirectToAction("Create", new { ClienteId = venta.ClienteId });
        }


    }
}
