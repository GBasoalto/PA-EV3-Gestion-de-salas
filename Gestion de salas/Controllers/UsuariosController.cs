using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Gestion_de_salas.Data;
using Gestion_de_salas.Models;

namespace Gestion_de_salas.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly DataContext _context;

        public UsuariosController(DataContext context)
        {
            _context = context;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            var dataContext = _context.Usuarios.Include(u => u.TipoUsuario);
            return View(await dataContext.ToListAsync());
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .Include(u => u.TipoUsuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            ViewData["TipoUsuarioId"] = new SelectList(_context.TipoUsuarios, "Id", "Tipo");
            return View();
        }

        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Usuario usuario) // Sin [Bind]
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(usuario);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al guardar: {ex.Message}");
                    ModelState.AddModelError("", "Error al guardar el usuario: " + ex.Message);
                }
            }

            ViewData["TipoUsuarioId"] = new SelectList(_context.TipoUsuarios, "Id", "Tipo");
            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            ViewData["TipoUsuarioId"] = new SelectList(_context.TipoUsuarios, "Id", "Tipo", usuario.TipoUsuarioId);
            return View(usuario);
        }


        // POST: Usuarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Usuario usuario, string NuevaPassword)
        {
            if (id != usuario.Id)
                return NotFound();

            // Quitar validaciones que no se usan
            ModelState.Remove("Password");
            ModelState.Remove("NuevaPassword");

            if (ModelState.IsValid)
            {
                try
                {
                    var usuarioDB = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id);

                    if (usuarioDB == null)
                        return NotFound();

                    // Actualizar datos
                    usuarioDB.Nombre = usuario.Nombre;
                    usuarioDB.Apellido1 = usuario.Apellido1;
                    usuarioDB.Apellido2 = usuario.Apellido2;
                    usuarioDB.Rut = usuario.Rut;
                    usuarioDB.Email = usuario.Email;
                    usuarioDB.Estado = usuario.Estado;
                    usuarioDB.TipoUsuarioId = usuario.TipoUsuarioId;

                    // Si el usuario ingresó nueva contraseña → actualizarla
                    if (!string.IsNullOrWhiteSpace(NuevaPassword))
                    {
                        usuarioDB.Password = NuevaPassword;
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al editar el usuario: " + ex.Message);
                }
            }

            ViewData["TipoUsuarioId"] =
                new SelectList(_context.TipoUsuarios, "Id", "Tipo", usuario.TipoUsuarioId);

            return View(usuario);
        }



        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .Include(u => u.TipoUsuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }
    }
}