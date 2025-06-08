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
                .Include(e => e.Equipo)
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
    }
}
