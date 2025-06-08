using Microsoft.AspNetCore.Mvc;
using TorneosBasketBall.Models;
using TorneosBasketBall.Servicios;

namespace TorneosBasketBall.Controllers
{
    public class HomeController : Controller
    {
        private readonly ContextoBD _context;

        public HomeController(ContextoBD context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Llenar el ViewModel con los datos de la base de datos
            var vm = new DashboardViewModel
            {
                Equipos = _context.Equipos.ToList(),
                Jugadores = _context.Jugadores.ToList(),
                Partidos = _context.Partidos.ToList(),
                Estadisticas = _context.EstadisticasEquipos.ToList()
            };
            return View(vm); // Busca Views/Home/Index.cshtml
        }
    }
}
