namespace Gestion_de_salas.Models
{
    public class Sala
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Capacidad { get; set; }
        public string EstadoSala { get; set; } = "Disponible"; // Valores: "Ocupada", "Disponible", "Mantenimiento"
        //RELACIÓN CON RESERVA
        public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    }
}
