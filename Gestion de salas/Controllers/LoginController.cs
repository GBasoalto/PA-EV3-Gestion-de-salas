using Gestion_de_salas.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gestion_de_salas.Controllers
{
    public class LoginController : Controller
    {
        private readonly DataContext _context;

        public LoginController(DataContext context)
        {
            _context = context;
        }

        // GET: /Login
        public IActionResult Index()
        {
            return View();
        }

        // POST: /Login
        [HttpPost]
        public async Task<IActionResult> Index(string rut, string password)
        {
            if (string.IsNullOrEmpty(rut) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Debe ingresar RUT y contraseña.";
                return View();
            }

            // Buscar usuario por RUT
            var usuario = await _context.Usuarios
                .Include(u => u.TipoUsuario)
                .FirstOrDefaultAsync(u => u.Rut == rut && u.Estado == true);

            if (usuario == null)
            {
                ViewBag.Error = "Usuario no encontrado o no activado.";
                return View();
            }

            // Verificar contraseña (por ahora sin hash)
            if (usuario.Password != password)
            {
                ViewBag.Error = "Rut o Contraseña incorrectas.";
                return View();
            }

            // Guardar datos de la sesion
            HttpContext.Session.SetInt32("UsuarioId", usuario.Id);
            HttpContext.Session.SetString("NombreUsuario", $"{usuario.Nombre} {usuario.Apellido1}");
            HttpContext.Session.SetString("TipoUsuario", usuario.TipoUsuario.Tipo.ToString());

            //REDIRIGIR AL DASHBOARD 
            return RedirectToAction("Index", "Home");
        }

        // Cerrar sesión
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }
    }
}
