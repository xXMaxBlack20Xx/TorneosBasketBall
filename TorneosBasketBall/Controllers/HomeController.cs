using Microsoft.AspNetCore.Mvc;
using TorneosBasketBall.Models;
using TorneosBasketBall.Servicios;

namespace TorneosBasketBall.Controllers
{
    public class HomeController : Controller
    {
        private readonly ContextoBD _context;
        private const int PageSize = 5;

        public HomeController(ContextoBD context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index(int? equipoId = null, int page = 1)
        {
            // 1. Traer todos los equipos
            var allEquipos = _context.Equipos.ToList();

            // 2. Filtrar jugadores, partidos y stats si se selecciona un equipo
            var allJugadores = _context.Jugadores.ToList();
            var allPartidos = _context.Partidos.ToList();
            var allStats = _context.EstadisticasEquipos.ToList();

            if (equipoId.HasValue && equipoId.Value > 0)
            {
                allJugadores = allJugadores
                    .Where(j => j.EquipoID == equipoId.Value).ToList();

                allPartidos = allPartidos
                    .Where(p => p.EquipoLocalID == equipoId.Value
                             || p.EquipoVisitanteID == equipoId.Value).ToList();

                allStats = allStats
                    .Where(e => e.EquipoID == equipoId.Value).ToList();
            }

            // 3. Paginación de equipos
            int totalEquipos = allEquipos.Count;
            int totalPages = (int)Math.Ceiling(totalEquipos / (double)PageSize);

            var pagedEquipos = allEquipos
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            // 4. Preparar ViewModel
            var vm = new DashboardViewModel
            {
                AllEquipos = allEquipos,
                Equipos = pagedEquipos,
                Jugadores = allJugadores,
                Partidos = allPartidos,
                Estadisticas = allStats
            };

            // 5. Pasar datos al View
            ViewBag.EquipoSeleccionado = equipoId ?? 0;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(vm);
        }
    }
}
