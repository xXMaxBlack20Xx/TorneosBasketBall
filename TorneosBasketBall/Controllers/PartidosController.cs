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
        public async Task<IActionResult> GenerateRoundRobin()
        {
            var equipos = await _context.Equipos.Select(e => e.EquipoID).ToListAsync();
            var roundRobinService = new RoundRobinService();
            var generatedMatches = roundRobinService.GenerateRoundRobin(equipos, DateTime.Now);

            _context.Partidos.AddRange(generatedMatches);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


    }

}
