using Microsoft.AspNetCore.Mvc;
using Ventas_MVC.Data;
using Ventas_MVC.Models;
using ClosedXML.Excel;
using System.Text.RegularExpressions;

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

        [HttpGet]
        public IActionResult Importar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Importar(IFormFile? archivo)
        {
            if (archivo == null || archivo.Length == 0)
            {
                ViewBag.Error = "Elegi un archivo Excel";
                return View();

            }
            if (!archivo.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.Error = "El archivo tiene que ser .xlsx";
                return View();
            }

            var existentes = new Dictionary<string, Cliente>(StringComparer.OrdinalIgnoreCase);
                foreach (var c in _contexto.Clientes.ToList())
            {
                existentes.TryAdd(Normalizar(c.Nombre), c);
            }

            int creados = 0, actualizados = 0, SinCambios = 0;
            var problemas = new List<string>();
            var vistos = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            using var libro = new XLWorkbook(archivo.OpenReadStream());
            var hoja = libro.Worksheet(1);

            foreach(var fila in hoja.RowsUsed().Skip(1))
            {
                var numero = fila.RowNumber();
                var nombre = Normalizar(fila.Cell(1).GetString());
                var direccion = Normalizar(fila.Cell(2).GetString());

                if (nombre == "")
                {
                    problemas.Add($"Fila{numero}: no tien nomnre.");
                    continue;
                }

                if (!vistos.Add(nombre))
                {
                    problemas.Add($"Fila{numero}: '{nombre}'está repetido en el archivo.");
                    continue;
                }

                if (existentes.TryGetValue(nombre, out var cliente))
                {
                    if (direccion != "" && cliente.Direccion != direccion)
                    {
                        cliente.Direccion = direccion;
                        actualizados++;
                    }
                    else
                        SinCambios++;

                }
                else
                {
                    _contexto.Clientes.Add(new Cliente { Nombre = nombre, Direccion = direccion });
                    creados++;
                }
            }

            _contexto.SaveChanges();

            ViewBag.Resultado = true;
            ViewBag.Creados = creados;
            ViewBag.Actualizados = actualizados;
            ViewBag.SinCambios = SinCambios;
            ViewBag.Problemas = problemas;
            return View();

        }
        private static string Normalizar(string? texto)
        {
            return Regex.Replace((texto ?? "").Trim(), @"\s+", "");
        }
    }
}
