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
    public class CanchaController : Controller
    {
        private readonly ContextoBD _context;

        public CanchaController(ContextoBD context)
        {
            _context = context;
        }

        // GET: Cancha
        public async Task<IActionResult> Index()
        {
            return View(await _context.Canchas.ToListAsync());
        }

        // GET: Cancha/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cancha = await _context.Canchas
                .FirstOrDefaultAsync(m => m.CanchaID == id);
            if (cancha == null)
            {
                return NotFound();
            }

            return View(cancha);
        }

        // GET: Cancha/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cancha/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CanchaID,Nombre,Direccion")] Cancha cancha)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cancha);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cancha);
        }

        // GET: Cancha/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cancha = await _context.Canchas.FindAsync(id);
            if (cancha == null)
            {
                return NotFound();
            }
            return View(cancha);
        }

        // POST: Cancha/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CanchaID,Nombre,Direccion")] Cancha cancha)
        {
            if (id != cancha.CanchaID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cancha);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CanchaExists(cancha.CanchaID))
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
            return View(cancha);
        }

        // GET: Cancha/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cancha = await _context.Canchas
                .FirstOrDefaultAsync(m => m.CanchaID == id);
            if (cancha == null)
            {
                return NotFound();
            }

            return View(cancha);
        }

        // POST: Cancha/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cancha = await _context.Canchas.FindAsync(id);
            if (cancha != null)
            {
                _context.Canchas.Remove(cancha);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CanchaExists(int id)
        {
            return _context.Canchas.Any(e => e.CanchaID == id);
        }
    }
}
