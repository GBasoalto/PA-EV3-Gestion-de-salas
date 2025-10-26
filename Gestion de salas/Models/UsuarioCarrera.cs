using System.ComponentModel.DataAnnotations.Schema;

namespace Gestion_de_salas.Models
{
    //TABLA INTERMEDIA ENTRE USUARIO Y CARRERA TIENE SU PROPIA ENTIDAD Y UTILIZA AMBOS MODELOS PARA LOS CAMBIOS ID DE USUARIO Y CARRERA
    public class UsuarioCarrera
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int CarreraId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; }


        [ForeignKey("CarreraId")]
        public Carrera Carrera { get; set; }
    }
}
