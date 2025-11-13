using Microsoft.EntityFrameworkCore;

using Gestion_de_salas.Models;


namespace Gestion_de_salas.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
            //identificar los modelos que se veran en la base de datos
            //public DbSet<Herramienta> Herramientas { get; set; } EJEMPLO!!
            public DbSet<Usuario> Usuarios { get; set; }
            public DbSet<TipoUsuario> TipoUsuarios { get; set; }
            public DbSet<Carrera> Carreras { get; set; }
            public DbSet<UsuarioCarrera> UsuarioCarreras { get; set; } 
            public DbSet<Sala> Salas { get; set; }
            public DbSet<Reserva> Reservas { get; set; }
            public DbSet<Movimiento> Movimientos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) //CREAR SEMILLAS ACA
        {
            base.OnModelCreating(modelBuilder);
            //EJEMPLOS DE SEMILLAS
            //modelBuilder.Entity<Rol>().HasData(
            //    new Rol { Id = 1, Nombre = "Administrador" },
            //    new Rol { Id = 2, Nombre = "Empleado" }
            //);

            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    Id = 1,
                    Nombre = "Gonzalo",
                    Apellido1 = "Bsaoalto",
                    Apellido2 = "Gallegos",
                    Rut = "15907638-5",
                    Email = "gbasoalto24@cftsa.cl",
                    Password = "12345678",
                    Estado = true,
                    TipoUsuarioId = 1 // Administrador
                },
                new Usuario
                {
                    Id = 2,
                    Nombre = "Jairo",
                    Apellido1 = "Castillo",
                    Apellido2 = "Vera",
                    Rut = "11111111-1",
                    Email = "jcastillo24@cftsa.cl",
                    Password = "12345678",
                    Estado = true,
                    TipoUsuarioId = 2 // Docente
                },
                new Usuario
                {
                    Id = 3,
                    Nombre = "Joel",
                    Apellido1 = "Castro",
                    Apellido2 = "Castro",
                    Rut = "22222222-2",
                    Email = "Jcastro24@cftsa.cl",
                    Estado = true,
                    Password =  "12345678",
                    TipoUsuarioId = 3 // Estudiante
                }
                );


                    modelBuilder.Entity<TipoUsuario>().HasData(
                new TipoUsuario { Id = 1, Tipo = TipoUsuario.Nombre.Administrador },
                new TipoUsuario { Id = 2, Tipo = TipoUsuario.Nombre.Docente },
                new TipoUsuario { Id = 3, Tipo = TipoUsuario.Nombre.Estudiante }
                );

            modelBuilder.Entity<Sala>().HasData(
                new Sala { Id = 1, Nombre = "Sala A", Capacidad = 4, EstadoSala = Sala.Estado.Disponible },
                new Sala { Id = 2, Nombre = "Sala B", Capacidad = 4, EstadoSala = Sala.Estado.Disponible },
                new Sala { Id = 3, Nombre = "Sala C", Capacidad = 4, EstadoSala = Sala.Estado.Disponible },
                new Sala { Id = 4, Nombre = "Sala D", Capacidad = 4, EstadoSala = Sala.Estado.Disponible }
                );

            modelBuilder.Entity<Carrera>().HasData(
                new Carrera { Id = 1, Nombre = "Técnico en Veterinaria" },
                new Carrera { Id = 2, Nombre = "Técnico en Farmacia" },
                new Carrera { Id = 3, Nombre = "Técnico en Mantenimiento Industrial" },
                new Carrera { Id = 4, Nombre = "Técnico en Desarrollo de Video Juegos" },
                new Carrera { Id = 5, Nombre = "Técnico en Comunicación Digital" },
                new Carrera { Id = 6, Nombre = "Técnico en Ciberseguridad" },
                new Carrera { Id = 7, Nombre = "Técnico en Cosmetología y Estética Integral" },
                new Carrera { Id = 8, Nombre = "Técnico en Logística" },
                new Carrera { Id = 9, Nombre = "Técnico en Masoterapia" },
                new Carrera { Id = 10, Nombre = "Técnico en Odontología" },
                new Carrera { Id = 11, Nombre = "Técnico en Enfermería" },
                new Carrera { Id = 12, Nombre = "Técnico en Párvulos y Básica" },
                new Carrera { Id = 13, Nombre = "Contabilidad General" },
                new Carrera { Id = 14, Nombre = "Administración de Empresas" },
                new Carrera { Id = 15, Nombre = "Técnico en Turismo" },
                new Carrera { Id = 16, Nombre = "Técnico en Trabajo Social" },
                new Carrera { Id = 17, Nombre = "Analista Programador" },
                new Carrera { Id = 18, Nombre = "Técnico en Proceso y Control de Calidad Alimentaria" },
                new Carrera { Id = 19, Nombre = "Técnico Agrícola" },
                new Carrera { Id = 20, Nombre = "Técnico en Vinicultura" },
                new Carrera { Id = 21, Nombre = "Prevención de Riesgos" },
                new Carrera { Id = 22, Nombre = "Técnico en Automatización y Control Industrial" },
                new Carrera { Id = 23, Nombre = "Técnico en Redes Eléctricas" },
                new Carrera { Id = 24, Nombre = "Topografía" },
                new Carrera { Id = 25, Nombre = "Técnico en Obras Civiles" },
                new Carrera { Id = 26, Nombre = "Técnico en Mecánica" },
                new Carrera { Id = 27, Nombre = "Técnico en Mecánica Automotriz y Autotrónica" }
                );
        }
    }
}
