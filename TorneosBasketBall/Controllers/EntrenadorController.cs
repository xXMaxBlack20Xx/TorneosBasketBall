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
    public class EntrenadorController : Controller
    {
        private readonly ContextoBD _context;

        public EntrenadorController(ContextoBD context)
        {
            _context = context;
        }

        // GET: Entrenador
        public async Task<IActionResult> Index()
        {
            return View(await _context.Entrenadores.ToListAsync());
        }

        // GET: Entrenador/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entrenador = await _context.Entrenadores
                .FirstOrDefaultAsync(m => m.EntrenadorID == id);
            if (entrenador == null)
            {
                return NotFound();
            }

            return View(entrenador);
        }

        // GET: Entrenador/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Entrenador/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EntrenadorID,NombreCompleto,Experiencia")] Entrenador entrenador)
        {
            if (ModelState.IsValid)
            {
                _context.Add(entrenador);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(entrenador);
        }

        // GET: Entrenador/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entrenador = await _context.Entrenadores.FindAsync(id);
            if (entrenador == null)
            {
                return NotFound();
            }
            return View(entrenador);
        }

        // POST: Entrenador/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EntrenadorID,NombreCompleto,Experiencia")] Entrenador entrenador)
        {
            if (id != entrenador.EntrenadorID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(entrenador);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EntrenadorExists(entrenador.EntrenadorID))
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
            return View(entrenador);
        }

        // GET: Entrenador/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entrenador = await _context.Entrenadores
                .FirstOrDefaultAsync(m => m.EntrenadorID == id);
            if (entrenador == null)
            {
                return NotFound();
            }

            return View(entrenador);
        }

        // POST: Entrenador/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entrenador = await _context.Entrenadores.FindAsync(id);
            if (entrenador != null)
            {
                _context.Entrenadores.Remove(entrenador);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EntrenadorExists(int id)
        {
            return _context.Entrenadores.Any(e => e.EntrenadorID == id);
        }
    }
}
