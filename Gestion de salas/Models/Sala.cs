namespace Gestion_de_salas.Models
{
    public class Sala
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Capacidad { get; set; }
        public Estado EstadoSala { get; set; }
        public enum Estado
        {
            Ocupada = 1,
            Disponible = 2,
            Mantenimiento = 3
        }

        //RELACIÓN CON RESERVA
        public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    }
}
