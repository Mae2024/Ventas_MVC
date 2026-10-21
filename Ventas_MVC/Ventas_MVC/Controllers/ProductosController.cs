using ClosedXML;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text.RegularExpressions;
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
            return RedirectToAction("Index");
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
                ViewBag.Error = "Elegí un archivo Excel (.xlsx).";
                return View();
            }

            if (!archivo.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.Error = "El archivo tiene que ser .xlsx.";
                return View();
            }

            var existentes = new Dictionary<string, Producto>(StringComparer.OrdinalIgnoreCase);
            foreach (var p in _contexto.Productos.ToList())
            {
                existentes.TryAdd(Normalizar(p.Nombre), p);
            }

            int creados = 0, actualizados = 0, sinCambios = 0;
            var problemas = new List<string>();
            var vistos = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            using var libro = new XLWorkbook(archivo.OpenReadStream());
            var hoja = libro.Worksheet(1);

            foreach (var fila in hoja.RowsUsed().Skip(1))
            {
                var numero = fila.RowNumber();
                var nombre = Normalizar(fila.Cell(1).GetString());

                if (nombre == "")
                {
                    problemas.Add($"Fila {numero}: no tiene descripción.");
                    continue;
                }

                if (!LeerPrecio(fila.Cell(3), out var precio) || precio <= 0)
                {
                    problemas.Add($"Fila {numero}: '{nombre}' no tiene un precio final válido.");
                    continue;
                }

                if (!vistos.Add(nombre))
                {
                    problemas.Add($"Fila {numero}: '{nombre}' está repetido en el archivo.");
                    continue;
                }

                if (existentes.TryGetValue(nombre, out var producto))
                {
                    if (producto.PrecioUnitario != precio)
                    {
                        producto.PrecioUnitario = precio;
                        actualizados++;
                    }
                    else
                    {
                        sinCambios++;
                    }
                }
                else
                {
                    _contexto.Productos.Add(new Producto { Nombre = nombre, PrecioUnitario = precio });
                    creados++;
                }
            }

            _contexto.SaveChanges();

            ViewBag.Resultado = true;
            ViewBag.Creados = creados;
            ViewBag.Actualizados = actualizados;
            ViewBag.SinCambios = sinCambios;
            ViewBag.Problemas = problemas;
            return View();
        }

        private static string Normalizar(string? texto)
        {
            return Regex.Replace((texto ?? "").Trim(), @"\s+", " ");
        }

        private static bool LeerPrecio(IXLCell celda, out decimal precio)
        {
            precio = 0;

            if (celda.DataType == XLDataType.Number)
            {
                precio = Math.Round((decimal)celda.GetDouble(), 2);
                return true;
            }

            var texto = celda.GetString().Replace("$", "").Trim();
            if (!decimal.TryParse(texto, NumberStyles.Number, CultureInfo.GetCultureInfo("es-AR"), out precio))
            {
                return false;
            }

            precio = Math.Round(precio, 2);
            return true;
        }
    }
}
    
