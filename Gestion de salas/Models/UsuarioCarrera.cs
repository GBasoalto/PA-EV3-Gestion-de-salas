using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gestion_de_salas.Models
{
    //TABLA INTERMEDIA ENTRE USUARIO Y CARRERA TIENE SU PROPIA ENTIDAD Y UTILIZA AMBOS MODELOS PARA LOS CAMBIOS ID DE USUARIO Y CARRERA
    public class UsuarioCarrera
    {
        public int Id { get; set; }
        
        [ForeignKey("Usuario")]
        public int UsuarioId { get; set; }

        [ValidateNever]
        public Usuario Usuario { get; set; }

        [ForeignKey("Carrera")]
        public int CarreraId { get; set; }

        [ValidateNever]
        public Carrera Carrera { get; set; }
    }
}
