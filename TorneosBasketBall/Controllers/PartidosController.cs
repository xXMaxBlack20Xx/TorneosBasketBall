using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TorneosBasketBall.Models;
using TorneosBasketBall.Servicios;

namespace TorneosBasketBall.Controllers
{
    public class PartidosController : Controller
    {
        private readonly ContextoBD _context;

        public PartidosController(ContextoBD context)
        {
            _context = context;
        }

        // GET: Partidos
        public async Task<IActionResult> Index()
        {
            return View(await _context.Partidos.ToListAsync());
        }

        // GET: Partidos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var partidos = await _context.Partidos
                .FirstOrDefaultAsync(m => m.PartidoID == id);
            if (partidos == null)
            {
                return NotFound();
            }

            return View(partidos);
        }

        // GET: Partidos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Partidos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PartidoID,EquipoLocalID,EquipoVisitanteID,FechaHora,Estado,PuntuacionLocal,PuntuacionVisitante")] Partidos partidos)
        {
            if (ModelState.IsValid)
            {
                _context.Add(partidos);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(partidos);
        }

        // GET: Partidos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var partidos = await _context.Partidos.FindAsync(id);
            if (partidos == null)
            {
                return NotFound();
            }
            return View(partidos);
        }

        // POST: Partidos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PartidoID,EquipoLocalID,EquipoVisitanteID,FechaHora,Estado,PuntuacionLocal,PuntuacionVisitante")] Partidos partidos)
        {
            if (id != partidos.PartidoID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(partidos);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PartidosExists(partidos.PartidoID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(partidos);
        }

        // GET: Partidos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var partidos = await _context.Partidos
                .FirstOrDefaultAsync(m => m.PartidoID == id);
            if (partidos == null)
            {
                return NotFound();
            }

            return View(partidos);
        }

        // POST: Partidos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var partidos = await _context.Partidos.FindAsync(id);
            if (partidos != null)
            {
                _context.Partidos.Remove(partidos);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PartidosExists(int id)
        {
            return _context.Partidos.Any(e => e.PartidoID == id);
        }
        // GET: Equipoes/GenerateRoundRobin
        public IActionResult GenerateRoundRobin()
        {
            // 1) Load all teams
            var teams = _context.Equipos
                                .OrderBy(e => e.EquipoID)
                                .ToList();

            // 2) Build a round‐robin schedule
            var rounds = CreateRoundRobinSchedule(teams);

            // 3) Simulate scores
            var rnd = new Random();
            var games = new List<Partidos>();
            for (int r = 0; r < rounds.Count; r++)
            {
                foreach (var match in rounds[r])
                {
                    games.Add(new Partidos
                    {
                        EquipoLocalID = match.Item1.EquipoID,
                        EquipoVisitanteID = match.Item2.EquipoID,
                        FechaHora = DateTime.Today.AddDays(r),
                        Estado = "Finalizado",
                        PuntuacionLocal = rnd.Next(50, 121),
                        PuntuacionVisitante = rnd.Next(50, 121)
                    });
                }
            }

            // 4) (Optional) write out a text file in wwwroot
            var file = Path.Combine(Directory.GetCurrentDirectory(),
                                    "wwwroot",
                                    "roundrobin_simulation.txt");
            using var writer = new StreamWriter(file);
            for (int i = 0; i < rounds.Count; i++)
            {
                writer.WriteLine($"--- Ronda {i + 1} ---");
                foreach (var match in rounds[i])
                {
                    var g = games[i * (teams.Count / 2) + rounds[i].IndexOf(match)];
                    writer.WriteLine($"{match.Item1.NombreEquipo} ({g.PuntuacionLocal}) vs " +
                                     $"{match.Item2.NombreEquipo} ({g.PuntuacionVisitante})");
                }
                writer.WriteLine();
            }

            // 5) Render a view listing them
            return View("SimulatedRoundRobin", games);
        }

        /// <summary>
        /// Simple “circle” algorithm for round-robin fixtures.
        /// </summary>
        private static List<List<(Equipo, Equipo)>> CreateRoundRobinSchedule(List<Equipo> teams)
        {
            if (teams.Count % 2 != 0)
                teams.Add(new Equipo { EquipoID = 0, NombreEquipo = "BYE" });

            int n = teams.Count;
            var schedule = new List<List<(Equipo, Equipo)>>();
            var rotating = teams.Skip(1).ToList();

            for (int round = 0; round < n - 1; round++)
            {
                var matches = new List<(Equipo, Equipo)>();
                matches.Add((teams[0], rotating.Last()));
                for (int i = 1; i < n / 2; i++)
                    matches.Add((rotating[i - 1], rotating[rotating.Count - 1 - i]));

                schedule.Add(matches);

                // rotate
                var last = rotating.Last();
                rotating.RemoveAt(rotating.Count - 1);
                rotating.Insert(0, last);
            }

            return schedule;
        }
    }

}
