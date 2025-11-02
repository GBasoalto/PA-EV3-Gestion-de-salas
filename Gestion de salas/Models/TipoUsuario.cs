namespace Gestion_de_salas.Models
{
    public class TipoUsuario
    {
        public int Id { get; set; }

        public Nombre Tipo { get; set; }
      

        //ESTABLECER LA RELACION CON USUARIO , UN USUARIO PUEDE TENER UN TIPO DE USUARIO, PERO UN TIPO DE USUARIO PUEDE TENER MUCHOS USUARIOS
        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();

        public enum Nombre
        {
            Administrador = 1,
            Docente = 2,
            Estudiante = 3
        }
    }
}
