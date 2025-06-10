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
    public class EquipoesController : Controller
    {
        private readonly ContextoBD _context;

        public EquipoesController(ContextoBD context)
        {
            _context = context;
        }

        // ======================
        // MÉTODO DE AUTORIZACIÓN
        // ======================
        private bool IsAdminLogged()
        {
            return HttpContext.Session.GetInt32("AdminID") != null;
        }

        // GET: Equipoes
        public async Task<IActionResult> Index()
        {
            if (!IsAdminLogged())
                return RedirectToAction("Index", "Login");

            return View(await _context.Equipos.ToListAsync());
        }

        // GET: Equipoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!IsAdminLogged())
                return RedirectToAction("Index", "Login");

            if (id == null)
            {
                return NotFound();
            }

            var equipo = await _context.Equipos
                .FirstOrDefaultAsync(m => m.EquipoID == id);
            if (equipo == null)
            {
                return NotFound();
            }

            return View(equipo);
        }

        // GET: Equipoes/Create
        public IActionResult Create()
        {
            if (!IsAdminLogged())
                return RedirectToAction("Index", "Login");

            return View();
        }

        // POST: Equipoes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EquipoID,NombreEquipo,EntrenadorID,NombreEntrenador,PartidoID")] Equipo equipo)
        {
            if (!IsAdminLogged())
                return RedirectToAction("Index", "Login");

            if (ModelState.IsValid)
            {
                _context.Add(equipo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(equipo);
        }

        // GET: Equipoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsAdminLogged())
                return RedirectToAction("Index", "Login");

            if (id == null)
            {
                return NotFound();
            }

            var equipo = await _context.Equipos.FindAsync(id);
            if (equipo == null)
            {
                return NotFound();
            }
            return View(equipo);
        }

        // POST: Equipoes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EquipoID,NombreEquipo,EntrenadorID,NombreEntrenador,PartidoID")] Equipo equipo)
        {
            if (!IsAdminLogged())
                return RedirectToAction("Index", "Login");

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
            return View(equipo);
        }

        // GET: Equipoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsAdminLogged())
                return RedirectToAction("Index", "Login");

            if (id == null)
            {
                return NotFound();
            }

            var equipo = await _context.Equipos
                .FirstOrDefaultAsync(m => m.EquipoID == id);
            if (equipo == null)
            {
                return NotFound();
            }

            return View(equipo);
        }

        // POST: Equipoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsAdminLogged())
                return RedirectToAction("Index", "Login");

            var equipo = await _context.Equipos.FindAsync(id);
            if (equipo == null)
                return NotFound();

            // Verifica si el equipo está relacionado con algún partido como local o visitante
            var tienePartidos = await _context.Partidos
                .AnyAsync(p => p.EquipoLocalID == id || p.EquipoVisitanteID == id);

            if (tienePartidos)
            {
                // Agrega un mensaje de error para mostrar en la vista
                ModelState.AddModelError("", "No puedes eliminar el equipo porque está relacionado con uno o más partidos.");
                return View("Delete", equipo); // Devuelve la vista de confirmación con el mensaje de error
            }

            _context.Equipos.Remove(equipo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool EquipoExists(int id)
        {
            return _context.Equipos.Any(e => e.EquipoID == id);
        }

        // Método para mostrar equipos en modo de solo lectura
        public async Task<IActionResult> ReadOnly()
        {
            var equipos = await _context.Equipos.ToListAsync();
            return View(equipos);
        }
    }
}
