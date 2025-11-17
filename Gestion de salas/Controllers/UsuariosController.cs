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
            var dataContext = _context.Usuarios
                .Include(u => u.TipoUsuario)
                .Include(u => u.UsuarioCarreras)
                    .ThenInclude(uc => uc.Carrera); // Incluir carreras

            return View(await dataContext.ToListAsync());
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var usuario = await _context.Usuarios
                .Include(u => u.TipoUsuario)
                .Include(u => u.UsuarioCarreras)
                    .ThenInclude(uc => uc.Carrera)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
                return NotFound();

            return View(usuario);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            ViewData["TipoUsuarioId"] = new SelectList(_context.TipoUsuarios, "Id", "Tipo");
            ViewData["Carreras"] = new MultiSelectList(_context.Carreras, "Id", "Nombre");
            return View();
        }

        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Usuario usuario, int[] selectedCarreras)
        {
            if (ModelState.IsValid)
            {
                // Agregar carreras seleccionadas
                if (selectedCarreras != null)
                {
                    foreach (var carreraId in selectedCarreras)
                    {
                        usuario.UsuarioCarreras.Add(new UsuarioCarrera
                        {
                            CarreraId = carreraId
                        });
                    }
                }

                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["TipoUsuarioId"] = new SelectList(_context.TipoUsuarios, "Id", "Tipo", usuario.TipoUsuarioId);
            ViewData["Carreras"] = new MultiSelectList(_context.Carreras, "Id", "Nombre", selectedCarreras);
            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var usuario = await _context.Usuarios
                .Include(u => u.UsuarioCarreras)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
                return NotFound();

            ViewData["TipoUsuarioId"] = new SelectList(_context.TipoUsuarios, "Id", "Tipo", usuario.TipoUsuarioId);
            ViewData["Carreras"] = new MultiSelectList(_context.Carreras, "Id", "Nombre",
                usuario.UsuarioCarreras.Select(uc => uc.CarreraId));

            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Usuario usuario, string NuevaPassword, int[] selectedCarreras)
        {
            if (id != usuario.Id)
                return NotFound();

            ModelState.Remove("Password");
            ModelState.Remove("NuevaPassword");

            if (ModelState.IsValid)
            {
                try
                {
                    var usuarioDB = await _context.Usuarios
                        .Include(u => u.UsuarioCarreras)
                        .FirstOrDefaultAsync(u => u.Id == id);

                    if (usuarioDB == null)
                        return NotFound();

                    // Actualizar campos
                    usuarioDB.Nombre = usuario.Nombre;
                    usuarioDB.Apellido1 = usuario.Apellido1;
                    usuarioDB.Apellido2 = usuario.Apellido2;
                    usuarioDB.Rut = usuario.Rut;
                    usuarioDB.Email = usuario.Email;
                    usuarioDB.Estado = usuario.Estado;
                    usuarioDB.TipoUsuarioId = usuario.TipoUsuarioId;

                    if (!string.IsNullOrWhiteSpace(NuevaPassword))
                    {
                        usuarioDB.Password = NuevaPassword;
                    }

                    // Actualizar carreras
                    usuarioDB.UsuarioCarreras.Clear();
                    if (selectedCarreras != null)
                    {
                        foreach (var carreraId in selectedCarreras)
                        {
                            usuarioDB.UsuarioCarreras.Add(new UsuarioCarrera
                            {
                                UsuarioId = id,
                                CarreraId = carreraId
                            });
                        }
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al editar el usuario: " + ex.Message);
                }
            }

            ViewData["TipoUsuarioId"] = new SelectList(_context.TipoUsuarios, "Id", "Tipo", usuario.TipoUsuarioId);
            ViewData["Carreras"] = new MultiSelectList(_context.Carreras, "Id", "Nombre", selectedCarreras);

            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var usuario = await _context.Usuarios
                .Include(u => u.TipoUsuario)
                .Include(u => u.UsuarioCarreras)
                    .ThenInclude(uc => uc.Carrera)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
                return NotFound();

            return View(usuario);
        }

        // POST: Usuarios/DeleteConfirmed/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }
    }
}
