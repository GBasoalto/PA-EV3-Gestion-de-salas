using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Gestion_de_salas.Data;
using Gestion_de_salas.Models;
using Microsoft.AspNetCore.Http;

namespace Gestion_de_salas.Controllers
{
    public class ReservasController : Controller
    {
        private readonly DataContext _context;

        public ReservasController(DataContext context)
        {
            _context = context;
        }

        // INDEX
        public async Task<IActionResult> Index()
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null) return RedirectToAction("Index", "Login");

            var tipoUsuario = HttpContext.Session.GetString("TipoUsuario");

            // Traer reservas incluyendo Sala y Usuario
            var reservas = await _context.Reservas
                .Include(r => r.Sala)
                .Include(r => r.Usuario)
                .ToListAsync();

            // Actualizar estado automático de reservas terminadas
            bool cambios = false;
            foreach (var r in reservas)
            {
                var fechaHoraFin = r.FechaReserva.ToDateTime(r.HoraFin);
                if (r.EstadoReserva == "Activa" && fechaHoraFin <= DateTime.Now)
                {
                    r.EstadoReserva = "Completada";
                    _context.Update(r);

                    // Registrar movimiento automático
                    var movimientoAuto = new Movimiento
                    {
                        ReservaId = r.Id,
                        Accion = "Completada",
                        Observacion = "Reserva finalizada automáticamente",
                        FechaMovimiento = DateTime.Now
                    };
                    _context.Movimientos.Add(movimientoAuto);

                    cambios = true;
                }
            }

            if (cambios)
            {
                await _context.SaveChangesAsync();
            }

            // Filtrar según tipo de usuario
            if (tipoUsuario != "Administrador")
            {
                reservas = reservas.Where(r => r.UsuarioId == usuarioId.Value).ToList();
            }

            return View(reservas);
        }

        // DETAILS
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var reserva = await _context.Reservas
                .Include(r => r.Sala)
                .Include(r => r.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);

            return reserva == null ? NotFound() : View(reserva);
        }

        // GET CREATE
        public IActionResult Create()
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null) return RedirectToAction("Index", "Login");

            var tipoUsuario = HttpContext.Session.GetString("TipoUsuario");

            ViewData["SalaId"] = new SelectList(_context.Salas.ToList(), "Id", "Nombre");

            if (tipoUsuario == "Administrador")
            {
                ViewData["Usuarios"] = _context.Usuarios
                    .Where(u => u.Estado)
                    .Select(u => new { u.Id, Nombre = u.Nombre + " " + u.Apellido1 })
                    .ToList();
            }

            var reserva = new Reserva
            {
                FechaReserva = DateOnly.FromDateTime(DateTime.Now),
                EstadoReserva = "Activa"
            };

            ViewData["MinFecha"] = DateOnly.FromDateTime(DateTime.Now).ToString("yyyy-MM-dd");

            return View(reserva);
        }

        // POST CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FechaReserva,HoraInicio,HoraFin,SalaId,UsuarioId")] Reserva reserva)
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null) return RedirectToAction("Index", "Login");

            var tipoUsuario = HttpContext.Session.GetString("TipoUsuario");

            if (tipoUsuario != "Administrador")
            {
                reserva.UsuarioId = usuarioId.Value;
            }
            else if (reserva.UsuarioId == 0)
            {
                ModelState.AddModelError("UsuarioId", "Debe seleccionar un usuario.");
            }

            reserva.EstadoReserva = "Activa";
            var hoy = DateOnly.FromDateTime(DateTime.Now);

            if (reserva.FechaReserva < hoy)
                ModelState.AddModelError("FechaReserva", "No se puede reservar en una fecha pasada.");

            var inicio = reserva.HoraInicio;
            var fin = reserva.HoraFin;
            var minutos = (int)(fin - inicio).TotalMinutes;

            if (minutos < 30 || minutos > 180)
                ModelState.AddModelError("", "La reserva debe durar entre 30 minutos y 3 horas.");

            if (!SalaDisponible(reserva.SalaId, reserva.FechaReserva, inicio, fin))
                ModelState.AddModelError("", "La sala ya está ocupada en ese horario.");

            if (!ModelState.IsValid)
            {
                ViewData["SalaId"] = new SelectList(_context.Salas, "Id", "Nombre");

                if (tipoUsuario == "Administrador")
                {
                    ViewData["Usuarios"] = _context.Usuarios
                        .Where(u => u.Estado)
                        .Select(u => new { u.Id, Nombre = u.Nombre + " " + u.Apellido1 })
                        .ToList();
                }
                return View(reserva);
            }

            _context.Add(reserva);
            await _context.SaveChangesAsync();

            // Registrar movimiento de creación
            var movimiento = new Movimiento
            {
                ReservaId = reserva.Id,
                Accion = "Creada",
                Observacion = $"Reserva creada por {(tipoUsuario == "Administrador" ? "Administrador" : "Usuario")}",
                FechaMovimiento = DateTime.Now
            };
            _context.Movimientos.Add(movimiento);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // EDIT GET
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva == null) return NotFound();

            ViewData["SalaId"] = new SelectList(_context.Salas, "Id", "Nombre");
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios.Where(u => u.Estado), "Id", "Nombre");

            return View(reserva);
        }

        // EDIT POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FechaReserva,HoraInicio,HoraFin,UsuarioId,SalaId,EstadoReserva")] Reserva reserva)
        {
            if (id != reserva.Id) return NotFound();

            var hoy = DateOnly.FromDateTime(DateTime.Now);

            if (reserva.FechaReserva < hoy)
                ModelState.AddModelError("FechaReserva", "No se puede colocar una fecha pasada.");

            if (!SalaDisponible(reserva.SalaId, reserva.FechaReserva, reserva.HoraInicio, reserva.HoraFin))
                ModelState.AddModelError("", "La sala ya está ocupada en ese horario.");

            if (!ModelState.IsValid)
            {
                ViewData["SalaId"] = new SelectList(_context.Salas, "Id", "Nombre");
                ViewData["UsuarioId"] = new SelectList(_context.Usuarios.Where(u => u.Estado), "Id", "Nombre");
                return View(reserva);
            }

            try
            {
                _context.Update(reserva);
                await _context.SaveChangesAsync();

                // Registrar movimiento de edición
                var movimientoEdit = new Movimiento
                {
                    ReservaId = reserva.Id,
                    Accion = "Editada",
                    Observacion = "Reserva modificada",
                    FechaMovimiento = DateTime.Now
                };
                _context.Movimientos.Add(movimientoEdit);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Reservas.Any(r => r.Id == reserva.Id)) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // DELETE
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var reserva = await _context.Reservas
                .Include(r => r.Sala)
                .Include(r => r.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);

            return reserva == null ? NotFound() : View(reserva);
        }

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

                // Registrar movimiento de cancelación
                var movimientoCancel = new Movimiento
                {
                    ReservaId = reserva.Id,
                    Accion = "Cancelada",
                    Observacion = "Reserva cancelada",
                    FechaMovimiento = DateTime.Now
                };
                _context.Movimientos.Add(movimientoCancel);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // DISPONIBILIDAD
        private bool SalaDisponible(int salaId, DateOnly fecha, TimeOnly inicio, TimeOnly fin)
        {
            return !_context.Reservas.Any(r =>
                r.SalaId == salaId &&
                r.FechaReserva == fecha &&
                r.EstadoReserva == "Activa" &&
                !(fin <= r.HoraInicio || inicio >= r.HoraFin)
            );
        }

        // AJAX BUSCAR USUARIOS
        [HttpGet]
        public JsonResult BuscarUsuarios(string filtro)
        {
            if (string.IsNullOrWhiteSpace(filtro))
                return Json(new List<object>());

            var usuarios = _context.Usuarios
                .Where(u =>
                    u.Nombre.Contains(filtro) ||
                    u.Apellido1.Contains(filtro) ||
                    u.Rut.Contains(filtro)
                )
                .Select(u => new
                {
                    id = u.Id,
                    nombre = u.Nombre + " " + u.Apellido1,
                    rut = u.Rut
                })
                .ToList();

            return Json(usuarios);
        }
    }
}
