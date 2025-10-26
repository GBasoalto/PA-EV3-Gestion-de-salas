namespace Gestion_de_salas.Models
{
    public class Sala
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Capacidad { get; set; }

        public enum Estado
        {
            Ocupada = 1,
            Disponible = 2,
            Mantenimiento = 3
        }
    }
}
