using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        private readonly Random _random = new Random();

        public PartidosController(ContextoBD context)
        {
            _context = context;
            _rr = new RoundRobinService();
        }

        // GET: /Partidos
        public async Task<IActionResult> Index()
        {
            var allPartidos = await _context.Partidos
                .Include(p => p.EquipoLocal)
                .Include(p => p.EquipoVisitante)
                .OrderBy(p => p.FechaHora)
                .ThenBy(p => p.PartidoID)
                .ToListAsync();

            Partidos finalGame = null;
            List<Partidos> otherGames = new();

            if (allPartidos.Any())
            {
                finalGame = allPartidos.Last();
                otherGames = allPartidos.Where(p => p.PartidoID != finalGame.PartidoID).ToList();
            }

            ViewBag.OtherGames = otherGames;
            ViewBag.FinalGame = finalGame;
            return View();
        }

        // POST: /Partidos/GenerateRoundRobin
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateRoundRobin()
        {
            _context.Partidos.RemoveRange(_context.Partidos);
            await _context.SaveChangesAsync();

            var teamIds = await _context.Equipos.Select(e => e.EquipoID).ToListAsync();
            var schedule = _rr.GenerateRoundRobin(teamIds, DateTime.Today);

            _context.Partidos.AddRange(schedule);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: /Partidos/ComputePlayoffs
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ComputePlayoffs()
        {
            // ... unchanged ...
            return RedirectToAction(nameof(Index));
        }

        // POST: /Partidos/ComputeFinal
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ComputeFinal()
        {
            // ... unchanged ...
            return RedirectToAction(nameof(Index));
        }

        // GET: /Partidos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var partido = await _context.Partidos.FindAsync(id);
            if (partido == null) return NotFound();

            var equipos = await _context.Equipos.OrderBy(e => e.NombreEquipo).ToListAsync();
            ViewBag.LocalList = new SelectList(equipos, "EquipoID", "NombreEquipo", partido.EquipoLocalID);
            ViewBag.VisitaList = new SelectList(equipos, "EquipoID", "NombreEquipo", partido.EquipoVisitanteID);
            return View(partido);
        }

        // POST: /Partidos/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PartidoID,EquipoLocalID,EquipoVisitanteID,FechaHora,Estado,PuntuacionLocal,PuntuacionVisitante")] Partidos posted)
        {
            if (id != posted.PartidoID)
                return NotFound();

            if (!ModelState.IsValid)
            {
                var equipos = await _context.Equipos.OrderBy(e => e.NombreEquipo).ToListAsync();
                ViewBag.LocalList = new SelectList(equipos, "EquipoID", "NombreEquipo", posted.EquipoLocalID);
                ViewBag.VisitaList = new SelectList(equipos, "EquipoID", "NombreEquipo", posted.EquipoVisitanteID);

                // Log binding errors for debugging
                foreach (var kv in ModelState)
                {
                    foreach (var err in kv.Value.Errors)
                    {
                        Console.WriteLine($"Binding error on '{kv.Key}': {err.ErrorMessage}");
                    }
                }

                return View(posted);
            }

            var existing = await _context.Partidos.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.EquipoLocalID = posted.EquipoLocalID;
            existing.EquipoVisitanteID = posted.EquipoVisitanteID;
            existing.FechaHora = posted.FechaHora;
            existing.Estado = posted.Estado;
            existing.PuntuacionLocal = posted.PuntuacionLocal;
            existing.PuntuacionVisitante = posted.PuntuacionVisitante;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Partidos.AnyAsync(e => e.PartidoID == id))
                    return NotFound();
                throw;
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
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
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
