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
    public class EstadisticasEquiposController : Controller
    {
        private readonly ContextoBD _context;

        public EstadisticasEquiposController(ContextoBD context)
        {
            _context = context;
        }

        // GET: EstadisticasEquipos
        public async Task<IActionResult> Index()
        {
            // Include the Equipo navigation property to get team names
            var contextoBD = _context.EstadisticasEquipos.Include(e => e.Equipo);
            return View(await contextoBD.ToListAsync());
        }

        // GET: EstadisticasEquipos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estadisticasEquipos = await _context.EstadisticasEquipos
                .Include(e => e.Equipo) // Include the Equipo navigation property
                .FirstOrDefaultAsync(m => m.EquipoID == id);
            if (estadisticasEquipos == null)
            {
                return NotFound();
            }

            return View(estadisticasEquipos);
        }

        // GET: EstadisticasEquipos/Create
        public IActionResult Create()
        {
            ViewData["EquipoID"] = new SelectList(_context.Equipos, "EquipoID", "NombreEquipo");
            return View();
        }

        // POST: EstadisticasEquipos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EquipoID,PartidosJugados,Ganados,Perdidos,PuntosFavor,PuntosContra")] EstadisticasEquipos estadisticasEquipos)
        {
            if (ModelState.IsValid)
            {
                _context.Add(estadisticasEquipos);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EquipoID"] = new SelectList(_context.Equipos, "EquipoID", "NombreEquipo", estadisticasEquipos.EquipoID);
            return View(estadisticasEquipos);
        }

        // GET: EstadisticasEquipos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estadisticasEquipos = await _context.EstadisticasEquipos.FindAsync(id);
            if (estadisticasEquipos == null)
            {
                return NotFound();
            }
            ViewData["EquipoID"] = new SelectList(_context.Equipos, "EquipoID", "NombreEquipo", estadisticasEquipos.EquipoID);
            return View(estadisticasEquipos);
        }

        // POST: EstadisticasEquipos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EquipoID,PartidosJugados,Ganados,Perdidos,PuntosFavor,PuntosContra")] EstadisticasEquipos estadisticasEquipos)
        {
            if (id != estadisticasEquipos.EquipoID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(estadisticasEquipos);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EstadisticasEquiposExists(estadisticasEquipos.EquipoID))
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
            ViewData["EquipoID"] = new SelectList(_context.Equipos, "EquipoID", "NombreEquipo", estadisticasEquipos.EquipoID);
            return View(estadisticasEquipos);
        }

        // GET: EstadisticasEquipos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estadisticasEquipos = await _context.EstadisticasEquipos
                .Include(e => e.Equipo)
                .FirstOrDefaultAsync(m => m.EquipoID == id);
            if (estadisticasEquipos == null)
            {
                return NotFound();
            }

            return View(estadisticasEquipos);
        }

        // POST: EstadisticasEquipos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var estadisticasEquipos = await _context.EstadisticasEquipos.FindAsync(id);
            if (estadisticasEquipos != null)
            {
                _context.EstadisticasEquipos.Remove(estadisticasEquipos);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EstadisticasEquiposExists(int id)
        {
            return _context.EstadisticasEquipos.Any(e => e.EquipoID == id);
        }

        // New action to compute and fill statistics
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ComputeStatistics()
        {
            // Clear existing statistics
            _context.EstadisticasEquipos.RemoveRange(_context.EstadisticasEquipos);
            await _context.SaveChangesAsync();

            // Get all teams
            var teams = await _context.Equipos.ToListAsync();

            foreach (var team in teams)
            {
                var teamId = team.EquipoID;

                // Calculate statistics for the current team
                var partidosJugados = await _context.Partidos
                    .CountAsync(p => p.Estado == "Finalizado" &&
                                     (p.EquipoLocalID == teamId || p.EquipoVisitanteID == teamId));

                var ganados = await _context.Partidos
                    .CountAsync(p => p.Estado == "Finalizado" &&
                                     ((p.EquipoLocalID == teamId && p.PuntuacionLocal > p.PuntuacionVisitante) ||
                                      (p.EquipoVisitanteID == teamId && p.PuntuacionVisitante > p.PuntuacionLocal)));

                var perdidos = await _context.Partidos
                    .CountAsync(p => p.Estado == "Finalizado" &&
                                     ((p.EquipoLocalID == teamId && p.PuntuacionLocal < p.PuntuacionVisitante) ||
                                      (p.EquipoVisitanteID == teamId && p.PuntuacionVisitante < p.PuntuacionLocal)));

                var puntosFavor = await _context.Partidos
                    .Where(p => p.Estado == "Finalizado" && p.EquipoLocalID == teamId)
                    .SumAsync(p => (int?)p.PuntuacionLocal) ?? 0;
                puntosFavor += await _context.Partidos
                    .Where(p => p.Estado == "Finalizado" && p.EquipoVisitanteID == teamId)
                    .SumAsync(p => (int?)p.PuntuacionVisitante) ?? 0;

                var puntosContra = await _context.Partidos
                    .Where(p => p.Estado == "Finalizado" && p.EquipoLocalID == teamId)
                    .SumAsync(p => (int?)p.PuntuacionVisitante) ?? 0;
                puntosContra += await _context.Partidos
                    .Where(p => p.Estado == "Finalizado" && p.EquipoVisitanteID == teamId)
                    .SumAsync(p => (int?)p.PuntuacionLocal) ?? 0;


                // Create or update EstadisticasEquipos entry
                var estadisticas = await _context.EstadisticasEquipos
                                         .FirstOrDefaultAsync(e => e.EquipoID == teamId);

                if (estadisticas == null)
                {
                    estadisticas = new EstadisticasEquipos
                    {
                        EquipoID = teamId,
                        Equipo = team // <--- FIX: Assign the 'team' object here
                    };
                    _context.EstadisticasEquipos.Add(estadisticas);
                }

                estadisticas.PartidosJugados = partidosJugados;
                estadisticas.Ganados = ganados;
                estadisticas.Perdidos = perdidos;
                estadisticas.PuntosFavor = puntosFavor;
                estadisticas.PuntosContra = puntosContra;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}