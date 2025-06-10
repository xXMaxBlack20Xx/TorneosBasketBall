using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TorneosBasketBall.Models;
using TorneosBasketBall.Services;
using TorneosBasketBall.Servicios;

namespace TorneosBasketBall.Controllers
{
    public class PartidosController : Controller
    {
        private readonly ContextoBD _context;
        private readonly RoundRobinService _rr;
        private Random _random = new Random();

        public PartidosController(ContextoBD context)
        {
            _context = context;
            _rr = new RoundRobinService();
        }

        // GET: /Partidos
        public async Task<IActionResult> Index()
        {
            // Eager load related Equipo data for display
            var allPartidos = await _context.Partidos
                                            .Include(p => p.EquipoLocal) // Load the local team data
                                            .Include(p => p.EquipoVisitante) // Load the visitor team data
                                            .OrderBy(p => p.FechaHora) // Order by date first
                                            .ThenBy(p => p.PartidoID) // Then by ID for consistent tie-breaking
                                            .ToListAsync();

            Partidos finalGame = null;
            List<Partidos> otherGames = new List<Partidos>();

            if (allPartidos.Any())
            {
                // The most recent game (by date, then ID) is assumed to be the final game.
                finalGame = allPartidos.LastOrDefault();

                // If a final game is found, exclude it from the list of "other games".
                if (finalGame != null)
                {
                    otherGames = allPartidos.Where(p => p.PartidoID != finalGame.PartidoID).ToList();
                }
                else
                {
                    otherGames = allPartidos; // Fallback: if no distinct final, all are "other"
                }
            }

            // Pass the separated lists to the view using ViewBag
            ViewBag.OtherGames = otherGames;
            ViewBag.FinalGame = finalGame;

            return View(); // Now passing data via ViewBag
        }

        // POST: /Partidos/GenerateRoundRobin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateRoundRobin()
        {
            // Clear ALL existing matches for a fresh start with Round Robin
            _context.Partidos.RemoveRange(_context.Partidos);
            await _context.SaveChangesAsync();

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
            if (top4.Count < 4) return BadRequest("Need 4 teams to compute playoffs.");

            // Heuristic for removing old playoff/final matches without an 'EtapaJuego' property.
            // This removes matches that are currently "Programado" OR have a date past a certain point
            // (assuming playoff/final matches are generated with future dates).
            var playoffThresholdDate = DateTime.Today.AddDays(standings.Count); // A rough estimate for when regular season ends
            var matchesToRemove = await _context.Partidos
                .Where(p => p.FechaHora >= playoffThresholdDate || p.Estado == "Programado")
                .ToListAsync();
            _context.Partidos.RemoveRange(matchesToRemove);
            await _context.SaveChangesAsync();

            // Semifinals
            var semis = new List<Partidos>();

            // Semifinal 1: Top 1 vs Top 4
            int homeScore1 = _random.Next(70, 120);
            int awayScore1 = _random.Next(70, 120);
            if (homeScore1 == awayScore1) homeScore1++; // Ensure a winner

            semis.Add(new Partidos
            {
                EquipoLocalID = top4[0],
                EquipoVisitanteID = top4[3],
                FechaHora = DateTime.Today.AddDays(standings.Count + 1), // Date after regular season
                Estado = "Finalizado",
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
                FechaHora = DateTime.Today.AddDays(standings.Count + 1), // Same date as Semi 1 for same round
                Estado = "Finalizado",
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
            // Clear any potentially existing "final" matches before adding a new one.
            // This assumes final matches have dates well after regular season matches.
            // Adjust the date threshold (e.g., DateTime.Today.AddDays(100)) if your dates vary widely.
            var futureMatchesToRemove = await _context.Partidos
                .Where(p => p.FechaHora >= DateTime.Today.AddDays(50)) // Removing games far in the future
                .ToListAsync();
            if (futureMatchesToRemove.Any()) // Only remove if there are matches matching the criteria
            {
                _context.Partidos.RemoveRange(futureMatchesToRemove);
                await _context.SaveChangesAsync();
            }

            // Get the two most recent finished matches to determine winners (assumed to be semifinals)
            var semis = await _context.Partidos
                .Where(p => p.Estado == "Finalizado")
                .OrderByDescending(p => p.FechaHora)
                .ThenByDescending(p => p.PartidoID)
                .Take(2) // Get the top 2 latest finished games
                .ToListAsync();

            if (semis.Count < 2) return BadRequest("2 finished semifinals needed to compute the final. Ensure they are finished and scored.");

            // Determine winners
            int w1 = semis[0].PuntuacionLocal > semis[0].PuntuacionVisitante ? (int)semis[0].EquipoLocalID : (int)semis[0].EquipoVisitanteID;
            int w2 = semis[1].PuntuacionLocal > semis[1].PuntuacionVisitante ? (int)semis[1].EquipoLocalID : (int)semis[1].EquipoVisitanteID;

            // Generate random scores for the final
            int homeScoreFinal = _random.Next(70, 120);
            int awayScoreFinal = _random.Next(70, 120);
            if (homeScoreFinal == awayScoreFinal) homeScoreFinal++; // Ensure a winner

            var final = new Partidos
            {
                EquipoLocalID = w1,
                EquipoVisitanteID = w2,
                // Set the date to be after the latest semifinal to ensure it appears last
                FechaHora = DateTime.Today.AddDays(semis.Max(s => (int)s.FechaHora.Subtract(DateTime.Today).TotalDays) + 1),
                Estado = "Finalizado",
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
            var p = await _context.Partidos
                                .Include(x => x.EquipoLocal)
                                .Include(x => x.EquipoVisitante)
                                .FirstOrDefaultAsync(m => m.PartidoID == id);
            if (p == null) return NotFound();
            return View(p);
        }

        // POST: /Partidos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Partidos partidos)
        {
            if (id != partidos.PartidoID) return NotFound();

            var existingPartido = await _context.Partidos.FindAsync(id);
            if (existingPartido == null) return NotFound();

            existingPartido.PuntuacionLocal = partidos.PuntuacionLocal;
            existingPartido.PuntuacionVisitante = partidos.PuntuacionVisitante;
            existingPartido.Estado = partidos.Estado;
            existingPartido.FechaHora = partidos.FechaHora;

            if (!ModelState.IsValid) return View(partidos);

            try
            {
                _context.Update(existingPartido);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Partidos.Any(e => e.PartidoID == id))
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

        // GET: /Partidos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var p = await _context.Partidos
                                .Include(x => x.EquipoLocal)
                                .Include(x => x.EquipoVisitante)
                                .FirstOrDefaultAsync(m => m.PartidoID == id);
            if (p == null) return NotFound();
            return View(p);
        }

        // POST: /Partidos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var p = await _context.Partidos.FindAsync(id);
            if (p != null)
            {
                _context.Partidos.Remove(p);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}