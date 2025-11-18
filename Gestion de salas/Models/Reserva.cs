using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Gestion_de_salas.Models
{
    public class Reserva
    {
        public int Id { get; set; }
        public DateOnly FechaReserva { get; set; }


        public TimeOnly HoraInicio { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una hora de fin")]
        public TimeOnly HoraFin { get; set; }
        [ForeignKey("Usuario")]
        public int UsuarioId { get; set; }

        [ValidateNever]
        public Usuario Usuario { get; set; }

        [ForeignKey("Sala")]
        public int SalaId { get; set; }

        [ValidateNever]
        public Sala Sala { get; set; }


        public string EstadoReserva { get; set; } = "Activa"; // Valores: "Activa", "Cancelada", "Completada"
        
        public List<Movimiento> Movimientos { get; set; } = new List<Movimiento>();
    }
}
