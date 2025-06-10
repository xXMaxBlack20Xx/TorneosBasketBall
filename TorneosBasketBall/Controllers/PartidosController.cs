using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TorneosBasketBall.Models;
using TorneosBasketBall.Services;
using TorneosBasketBall.Servicios;
// using TorneosBasketBall.Servicios; // This line might be redundant if not used

namespace TorneosBasketBall.Controllers
{
    public class PartidosController : Controller
    {
        private readonly ContextoBD _context;
        private readonly RoundRobinService _rr;
        private Random _random = new Random(); // Add this for random playoff scores

        public PartidosController(ContextoBD context)
        {
            _context = context;
            _rr = new RoundRobinService();
        }

        // GET: /Partidos
        public async Task<IActionResult> Index()
        {
            var all = await _context.Partidos.OrderBy(p => p.FechaHora).ToListAsync();
            return View(all);
        }

        // POST: /Partidos/GenerateRoundRobin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateRoundRobin()
        {
            // clear existing
            _context.Partidos.RemoveRange(_context.Partidos);
            await _context.SaveChangesAsync();

            // generate new
            var teamIds = await _context.Equipos.Select(e => e.EquipoID).ToListAsync();
            var schedule = _rr.GenerateRoundRobin(teamIds, DateTime.Today);
            _context.Partidos.AddRange(schedule);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // POST: /Partidos/ComputePlayoffs
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ComputePlayoffs()
        {
            // compute points (2=win,1=tie)
            var standings = await _context.Equipos.Select(e => new {
                e.EquipoID,
                Points = _context.Partidos.Count(p => p.Estado == "Finalizado"
                    && ((p.EquipoLocalID == e.EquipoID && p.PuntuacionLocal > p.PuntuacionVisitante)
                      || (p.EquipoVisitanteID == e.EquipoID && p.PuntuacionVisitante > p.PuntuacionLocal))) * 2
                    + _context.Partidos.Count(p => p.Estado == "Finalizado"
                    && p.PuntuacionLocal == p.PuntuacionVisitante
                    && (p.EquipoLocalID == e.EquipoID || p.EquipoVisitanteID == e.EquipoID))
            }).OrderByDescending(x => x.Points).ToListAsync();

            var top4 = standings.Select(x => x.EquipoID).Take(4).ToList();
            if (top4.Count < 4) return BadRequest("Need 4 teams.");

            // remove old playoff matches
            // This logic is slightly off, it removes future matches based on a dynamic date
            // A more robust way might be to add a 'Stage' property to Partidos (e.g., "Regular", "SemiFinal", "Final")
            // For now, keeping your existing logic but noting it could be refined.
            _context.Partidos.RemoveRange(
                _context.Partidos.Where(p => p.FechaHora > DateTime.Today.AddDays(standings.Count) || p.Estado == "Programado")); // Added removal of "Programado" matches
            await _context.SaveChangesAsync();

            // semifinals
            var semis = new List<Partidos>();

            // Semifinal 1: Top 1 vs Top 4
            int homeScore1 = _random.Next(70, 120);
            int awayScore1 = _random.Next(70, 120);
            if (homeScore1 == awayScore1) homeScore1++; // Ensure a winner

            semis.Add(new Partidos
            {
                EquipoLocalID = top4[0],
                EquipoVisitanteID = top4[3],
                FechaHora = DateTime.Today.AddDays(standings.Count + 1),
                Estado = "Finalizado", // Automatically mark as Finalizado
                PuntuacionLocal = homeScore1,
                PuntuacionVisitante = awayScore1
            });

            // Semifinal 2: Top 2 vs Top 3
            int homeScore2 = _random.Next(70, 120);
            int awayScore2 = _random.Next(70, 120);
            if (homeScore2 == awayScore2) homeScore2++; // Ensure a winner

            semis.Add(new Partidos
            {
                EquipoLocalID = top4[1],
                EquipoVisitanteID = top4[2],
                FechaHora = DateTime.Today.AddDays(standings.Count + 1),
                Estado = "Finalizado", // Automatically mark as Finalizado
                PuntuacionLocal = homeScore2,
                PuntuacionVisitante = awayScore2
            });

            _context.Partidos.AddRange(semis);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // POST: /Partidos/ComputeFinal
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ComputeFinal()
        {
            var semis = await _context.Partidos
                .Where(p => p.Estado == "Finalizado")
                .OrderByDescending(p => p.FechaHora)
                .Take(2)
                .ToListAsync();

            // Check if there are exactly 2 semifinals to compute the final
            if (semis.Count < 2) return BadRequest("2 semifinals needed to compute the final. Ensure they are finished and scored.");

            // Determine winners
            int w1 = semis[0].PuntuacionLocal > semis[0].PuntuacionVisitante ? (int)semis[0].EquipoLocalID : (int)semis[0].EquipoVisitanteID;
            int w2 = semis[1].PuntuacionLocal > semis[1].PuntuacionVisitante ? (int)semis[1].EquipoLocalID : (int)semis[1].EquipoVisitanteID;

            // Generate random scores for the final
            int homeScoreFinal = _random.Next(70, 120);
            int awayScoreFinal = _random.Next(70, 120);
            if (homeScoreFinal == awayScoreFinal) homeScoreFinal++; // Ensure a winner for the final

            var final = new Partidos
            {
                EquipoLocalID = w1,
                EquipoVisitanteID = w2,
                FechaHora = DateTime.Today.AddDays(semis.Count + 2), // Adjust date if needed
                Estado = "Finalizado", // Automatically mark as Finalizado
                PuntuacionLocal = homeScoreFinal,
                PuntuacionVisitante = awayScoreFinal
            };
            _context.Partidos.Add(final);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Partidos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var p = await _context.Partidos.FindAsync(id);
            if (p == null) return NotFound();
            return View(p);
        }

        // POST: /Partidos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Partidos partidos)
        {
            if (id != partidos.PartidoID) return NotFound();
            if (!ModelState.IsValid) return View(partidos);
            _context.Update(partidos);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Partidos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var p = await _context.Partidos.FirstOrDefaultAsync(m => m.PartidoID == id);
            if (p == null) return NotFound();
            return View(p);
        }

        // POST: /Partidos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var p = await _context.Partidos.FindAsync(id);
            _context.Partidos.Remove(p);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}