using System.ComponentModel.DataAnnotations.Schema;

namespace Gestion_de_salas.Models
{
    public class Reserva
    {
        public int Id { get; set; }
        public DateOnly FechaReserva { get; set; }
        public TimeOnly HoraInicio { get; set; }
        public TimeOnly HoraFin { get; set; }
        [ForeignKey("Usuario")]
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        [ForeignKey("Sala")]
        public int SalaId { get; set; }
        public Sala Sala { get; set; }


        public Estado EstadoReserva { get; set; }
        public enum Estado
        {
            Activa = 1,
            Cancelada = 2,
            Completada = 3
        }
        public List<Movimiento> Movimientos { get; set; } = new List<Movimiento>();
    }
}
