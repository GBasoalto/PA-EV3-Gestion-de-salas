namespace Gestion_de_salas.Models
{
    public class Carrera
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        //RELACIÓN CON TABLA INTERMEDIA USUARIOCARRERA
        public List<UsuarioCarrera> UsuarioCarreras { get; set; } = new List<UsuarioCarrera>();
    }
}

