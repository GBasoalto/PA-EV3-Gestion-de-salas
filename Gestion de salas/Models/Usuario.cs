using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gestion_de_salas.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido1 { get; set; }
        public string Apellido2 { get; set; }
        public string Rut { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool Estado { get; set; }

        //ESTABLECER LA RELACION CON TIPOUSUARIO
        [Required(ErrorMessage = "El tipo de usuario es obligatorio")]
        [ForeignKey("TipoUsuario")]
        public int TipoUsuarioId { get; set; }

        [ValidateNever]
        public TipoUsuario TipoUsuario { get; set; }


        // RELACIÓN CON TABLA INTERMEDIA USUARIOCARRERA
        public List<UsuarioCarrera> UsuarioCarreras { get; set; } = new List<UsuarioCarrera>();
    }
}
