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
    public class EquiposController : Controller
    {
        private readonly ContextoBD _context;

        public EquiposController(ContextoBD context)
        {
            _context = context;
        }

        // GET: Equipos
        public async Task<IActionResult> Index()
        {
            var contextoBD = _context.Equipos.Include(e => e.Entrenador).Include(e => e.Torneo);
            return View(await contextoBD.ToListAsync());
        }

        // GET: Equipos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipo = await _context.Equipos
                .Include(e => e.Entrenador)
                .Include(e => e.Torneo)
                .FirstOrDefaultAsync(m => m.EquipoID == id);
            if (equipo == null)
            {
                return NotFound();
            }

            return View(equipo);
        }

        // GET: Equipos/Create
        public IActionResult Create()
        {
            ViewData["EntrenadorID"] = new SelectList(_context.Entrenadores, "EntrenadorID", "NombreCompleto");
            ViewData["TorneoID"] = new SelectList(_context.Torneos, "TorneoID", "Nombre");
            return View();
        }

        // POST: Equipos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EquipoID,NombreEquipo,EntrenadorID,TorneoID")] Equipo equipo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(equipo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EntrenadorID"] = new SelectList(_context.Entrenadores, "EntrenadorID", "NombreCompleto", equipo.EntrenadorID);
            ViewData["TorneoID"] = new SelectList(_context.Torneos, "TorneoID", "Nombre", equipo.TorneoID);
            return View(equipo);
        }

        // GET: Equipos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipo = await _context.Equipos.FindAsync(id);
            if (equipo == null)
            {
                return NotFound();
            }
            ViewData["EntrenadorID"] = new SelectList(_context.Entrenadores, "EntrenadorID", "NombreCompleto", equipo.EntrenadorID);
            ViewData["TorneoID"] = new SelectList(_context.Torneos, "TorneoID", "Nombre", equipo.TorneoID);
            return View(equipo);
        }

        // POST: Equipos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EquipoID,NombreEquipo,EntrenadorID,TorneoID")] Equipo equipo)
        {
            if (id != equipo.EquipoID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(equipo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EquipoExists(equipo.EquipoID))
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
            ViewData["EntrenadorID"] = new SelectList(_context.Entrenadores, "EntrenadorID", "NombreCompleto", equipo.EntrenadorID);
            ViewData["TorneoID"] = new SelectList(_context.Torneos, "TorneoID", "Nombre", equipo.TorneoID);
            return View(equipo);
        }

        // GET: Equipos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipo = await _context.Equipos
                .Include(e => e.Entrenador)
                .Include(e => e.Torneo)
                .FirstOrDefaultAsync(m => m.EquipoID == id);
            if (equipo == null)
            {
                return NotFound();
            }

            return View(equipo);
        }

        // POST: Equipos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var equipo = await _context.Equipos.FindAsync(id);
            if (equipo != null)
            {
                _context.Equipos.Remove(equipo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EquipoExists(int id)
        {
            return _context.Equipos.Any(e => e.EquipoID == id);
        }
    }
}
