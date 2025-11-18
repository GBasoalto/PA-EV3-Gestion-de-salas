using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Gestion_de_salas.Data;
using Gestion_de_salas.Models;

namespace Gestion_de_salas.Controllers
{
    public class MovimientoesController : Controller
    {
        private readonly DataContext _context;

        public MovimientoesController(DataContext context)
        {
            _context = context;
        }

        // Verificar si el usuario es admin
        private bool EsAdmin() => HttpContext.Session.GetString("TipoUsuario") == "Administrador";

        // GET: Movimientoes
        public async Task<IActionResult> Index()
        {
            if (!EsAdmin()) return Forbid();

            var movimientos = _context.Movimientos
                .Include(m => m.Reserva)
                .ThenInclude(r => r.Usuario)
                .Include(m => m.Reserva)
                .ThenInclude(r => r.Sala)
                .OrderByDescending(m => m.FechaMovimiento);

            return View(await movimientos.ToListAsync());
        }

        // GET: Movimientoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!EsAdmin()) return Forbid();
            if (id == null) return NotFound();

            var movimiento = await _context.Movimientos
                .Include(m => m.Reserva)
                .ThenInclude(r => r.Usuario)
                .Include(m => m.Reserva)
                .ThenInclude(r => r.Sala)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movimiento == null) return NotFound();

            return View(movimiento);
        }

        // GET: Movimientoes/Create
        public IActionResult Create(int? reservaId)
        {
            if (!EsAdmin()) return Forbid();

            if (reservaId != null)
            {
                ViewData["ReservaId"] = new SelectList(_context.Reservas.Where(r => r.Id == reservaId), "Id", "Id", reservaId);
            }
            else
            {
                ViewData["ReservaId"] = new SelectList(_context.Reservas, "Id", "Id");
            }

            return View();
        }

        // POST: Movimientoes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReservaId,Accion,Observacion")] Movimiento movimiento)
        {
            if (!EsAdmin()) return Forbid();

            if (ModelState.IsValid)
            {
                movimiento.FechaMovimiento = DateTime.Now;
                _context.Add(movimiento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ReservaId"] = new SelectList(_context.Reservas, "Id", "Id", movimiento.ReservaId);
            return View(movimiento);
        }

        // No permitir editar ni eliminar movimientos
        // GET / POST Edit eliminados
        // GET / POST Delete eliminados

        private bool MovimientoExists(int id)
        {
            return _context.Movimientos.Any(e => e.Id == id);
        }
    }
}
