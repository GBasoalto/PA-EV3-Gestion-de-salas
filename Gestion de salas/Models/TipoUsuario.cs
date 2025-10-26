namespace Gestion_de_salas.Models
{
    public class TipoUsuario
    {
        public int Id { get; set; }
        public enum Nombre
        {
            Administrador = 1,
            Docente = 2,
            Estudiante = 3
        }

        //ESTABLECER LA RELACION CON USUARIO , UN USUARIO PUEDE TENER UN TIPO DE USUARIO, PERO UN TIPO DE USUARIO PUEDE TENER MUCHOS USUARIOS
        public ICollection<Usuario> Usuarios { get; set; }
    }
}
