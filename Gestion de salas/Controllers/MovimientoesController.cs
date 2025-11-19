using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gestion_de_salas.Data;
using Gestion_de_salas.Models;
using Microsoft.AspNetCore.Http;

namespace Gestion_de_salas.Controllers
{
    public class MovimientoesController : Controller
    {
        private readonly DataContext _context;

        public MovimientoesController(DataContext context)
        {
            _context = context;
        }

        // Helper para verificar admin
        private bool EsAdmin()
        {
            var tipoUsuario = HttpContext.Session.GetString("TipoUsuario");
            return tipoUsuario == "Administrador";
        }

        // GET: Movimientoes
        public async Task<IActionResult> Index()
        {
            if (!EsAdmin()) return Forbid();

            var movimientos = _context.Movimientos
                .Include(m => m.Reserva)
                    .ThenInclude(r => r.Usuario)
                        .ThenInclude(u => u.UsuarioCarreras)
                            .ThenInclude(uc => uc.Carrera)
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
                        .ThenInclude(u => u.UsuarioCarreras)
                            .ThenInclude(uc => uc.Carrera)
                .Include(m => m.Reserva)
                    .ThenInclude(r => r.Sala)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movimiento == null) return NotFound();

            return View(movimiento);
        }

        // GET: Movimientoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!EsAdmin()) return Forbid(); // solo admin
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


        // POST: Movimientoes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Observacion")] Movimiento movimiento)
        {
            if (!EsAdmin()) return Forbid();

            // 1. Buscamos el original en la BD
            var movimientoOriginal = await _context.Movimientos
                .Include(m => m.Reserva)
                .ThenInclude(r => r.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movimientoOriginal == null)
            {
                return NotFound();
            }

            try
            {
                movimientoOriginal.Observacion = movimiento.Observacion;

                _context.Update(movimientoOriginal);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Movimientos.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            
        }

    }
}
