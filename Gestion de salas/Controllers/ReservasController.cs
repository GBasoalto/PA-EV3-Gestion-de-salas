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
    public class ReservasController : Controller
    {
        private readonly DataContext _context;


        public ReservasController(DataContext context)
        {
            _context = context;
        }

        // GET: Reservas
        public async Task<IActionResult> Index()
        {
            var dataContext = _context.Reservas.Include(r => r.Sala).Include(r => r.Usuario);
            return View(await dataContext.ToListAsync());
        }

        // GET: Reservas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var reserva = await _context.Reservas
                .Include(r => r.Sala)
                .Include(r => r.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (reserva == null) return NotFound();

            return View(reserva);
        }

        // GET: Reservas/Create
        public IActionResult Create()
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null) return RedirectToAction("Index", "Login");

            ViewData["UsuarioId"] = new SelectList(_context.Usuarios.Where(u => u.Id == usuarioId), "Id", "Nombre");
            ViewData["SalaId"] = new SelectList(_context.Salas, "Id", "Nombre");

            var reserva = new Reserva
            {
                FechaReserva = DateOnly.FromDateTime(DateTime.Now),
                HoraInicio = TimeOnly.FromDateTime(DateTime.Now.AddHours(1)),
                HoraFin = TimeOnly.FromDateTime(DateTime.Now.AddHours(2)),
                EstadoReserva = "Activa"
            };

            return View(reserva);
        }

        // POST: Reservas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FechaReserva,HoraInicio,HoraFin,SalaId")] Reserva reserva)
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null) return RedirectToAction("Index", "Login");

            reserva.UsuarioId = usuarioId.Value;
            reserva.EstadoReserva = "Activa";

            // Validar duración mínima 30 min y máxima 3 horas
            if ((reserva.HoraFin - reserva.HoraInicio).TotalMinutes < 30 ||
                (reserva.HoraFin - reserva.HoraInicio).TotalHours > 3)
            {
                ModelState.AddModelError("", "La duración de la reserva debe ser entre 30 minutos y 3 horas.");
            }

            // Verificar solapamiento
            if (!SalaDisponible(reserva.SalaId, reserva.FechaReserva, reserva.HoraInicio, reserva.HoraFin))
            {
                ModelState.AddModelError("", "La sala ya está ocupada en ese horario.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(reserva);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["UsuarioId"] = new SelectList(_context.Usuarios.Where(u => u.Id == usuarioId), "Id", "Nombre", reserva.UsuarioId);
            ViewData["SalaId"] = new SelectList(_context.Salas, "Id", "Nombre", reserva.SalaId);
            return View(reserva);
        }

        // GET: Reservas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva == null) return NotFound();

            LlenarDropdowns();
            return View(reserva);
        }

        // POST: Reservas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FechaReserva,HoraInicio,HoraFin,UsuarioId,SalaId,EstadoReserva")] Reserva reserva)
        {
            if (id != reserva.Id) return NotFound();

            // Validaciones de horario y duración
            var usuario = await _context.Usuarios.Include(u => u.TipoUsuario).FirstOrDefaultAsync(u => u.Id == reserva.UsuarioId);
            if (usuario == null)
            {
                ModelState.AddModelError("", "Usuario no válido.");
                LlenarDropdowns();
                return View(reserva);
            }

            if (!EsDiaValido(reserva.FechaReserva))
                ModelState.AddModelError("", "No se puede reservar en domingos o festivos.");

            if (!EsHorarioValido(reserva.HoraInicio, reserva.HoraFin))
                ModelState.AddModelError("", "El horario debe estar entre 09:00 y 21:00 y HoraFin > HoraInicio.");

            if (!DuracionValida(reserva.HoraInicio, reserva.HoraFin, usuario.TipoUsuarioId))
                ModelState.AddModelError("", "La duración debe ser entre 30 minutos y 3 horas para estudiantes.");

            if (usuario.TipoUsuarioId == 3 && !PuedeReservarEstudiante(usuario.Id, reserva.FechaReserva, reserva.HoraInicio, reserva.HoraFin))
                ModelState.AddModelError("", "Los estudiantes solo pueden reservar hasta 3 horas por día.");

            if (!SalaDisponible(reserva.SalaId, reserva.FechaReserva, reserva.HoraInicio, reserva.HoraFin))
                ModelState.AddModelError("", "La sala ya está ocupada en ese horario.");

            if (!ModelState.IsValid)
            {
                LlenarDropdowns();
                return View(reserva);
            }

            try
            {
                _context.Update(reserva);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReservaExists(reserva.Id)) return NotFound();
                else throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Reservas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var reserva = await _context.Reservas
                .Include(r => r.Sala)
                .Include(r => r.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (reserva == null) return NotFound();

            return View(reserva);
        }

        // POST: Reservas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva != null)
            {
                reserva.EstadoReserva = "Cancelada";
                _context.Update(reserva);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ReservaExists(int id)
        {
            return _context.Reservas.Any(e => e.Id == id);
        }

        #region Helpers y validaciones

        private void LlenarDropdowns()
        {
            var usuarios = _context.Usuarios
                .Where(u => u.Estado)
                .Select(u => new { u.Id, NombreCompleto = u.Nombre + " " + u.Apellido1 })
                .ToList();

            var salas = _context.Salas
                .Select(s => new { s.Id, s.Nombre })
                .ToList();

            ViewBag.UsuarioId = new SelectList(usuarios, "Id", "NombreCompleto");
            ViewBag.SalaId = new SelectList(salas, "Id", "Nombre");
        }

        private bool EsDiaValido(DateOnly fecha)
        {
            if (fecha.DayOfWeek == DayOfWeek.Sunday) return false;

            List<DateOnly> festivos = new List<DateOnly>
            {
                new DateOnly(2025,1,1),
                new DateOnly(2025,5,1),
                new DateOnly(2025,9,18),
                new DateOnly(2025,12,25)
            };
            return !festivos.Contains(fecha);
        }

        private bool EsHorarioValido(TimeOnly inicio, TimeOnly fin)
        {
            var apertura = new TimeOnly(9, 0);
            var cierre = new TimeOnly(21, 0);
            return inicio >= apertura && fin <= cierre && fin > inicio;
        }

        private bool DuracionValida(TimeOnly inicio, TimeOnly fin, int tipoUsuarioId)
        {
            int duracion = (int)(fin - inicio).TotalMinutes;
            if (duracion < 30) return false;
            if (tipoUsuarioId == 3 && duracion > 180) return false;
            return true;
        }

        private bool PuedeReservarEstudiante(int usuarioId, DateOnly fecha, TimeOnly inicio, TimeOnly fin)
        {
            var totalMinutos = _context.Reservas
                .Where(r => r.UsuarioId == usuarioId && r.FechaReserva == fecha)
                .Sum(r => (int)(r.HoraFin - r.HoraInicio).TotalMinutes);

            int nuevosMinutos = (int)(fin - inicio).TotalMinutes;
            return (totalMinutos + nuevosMinutos) <= 180;
        }

        private bool SalaDisponible(int salaId, DateOnly fecha, TimeOnly inicio, TimeOnly fin)
        {
            return !_context.Reservas.Any(r =>
                r.SalaId == salaId &&
                r.FechaReserva == fecha &&
                r.EstadoReserva == "Activa" &&
                ((inicio >= r.HoraInicio && inicio < r.HoraFin) ||
                 (fin > r.HoraInicio && fin <= r.HoraFin) ||
                 (inicio <= r.HoraInicio && fin >= r.HoraFin))
            );
        }

        // AJAX: obtener salas disponibles
        [HttpGet]
        public IActionResult GetDisponibilidad(DateOnly fecha, TimeOnly? horaInicio = null, TimeOnly? horaFin = null)
        {
            var salas = _context.Salas.ToList();
            var reservas = _context.Reservas
                .Where(r => r.FechaReserva == fecha && r.EstadoReserva == "Activa")
                .ToList();

            if (horaInicio.HasValue && horaFin.HasValue)
            {
                reservas = reservas
                    .Where(r => !(horaFin.Value <= r.HoraInicio || horaInicio.Value >= r.HoraFin))
                    .ToList();
            }

            var disponibles = salas.Where(s => !reservas.Any(r => r.SalaId == s.Id)).ToList();

            return Json(new { salas = disponibles });
        }

        #endregion
    }
}
