using System.ComponentModel.DataAnnotations.Schema;

namespace Gestion_de_salas.Models
{
    public class Movimiento
    {
        public int Id { get; set; }
        [ForeignKey("Reserva")]
        public int ReservaId { get; set; }

        public DateTime FechaMovimiento { get; set; } = DateTime.Now;

        public string Accion { get; set; }

        public string Observacion { get; set; }

        public Reserva Reserva { get; set; }
    }
}
