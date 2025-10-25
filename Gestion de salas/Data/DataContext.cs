using Microsoft.EntityFrameworkCore;

namespace Gestion_de_salas.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            //identificar los modelos que se veran en la base de datos
            //public DbSet<Herramienta> Herramientas { get; set; } EJEMPLO!!

    }
        protected override void OnModelCreating(ModelBuilder modelBuilder) //CREAR SEMILLAS ACA
        {
            base.OnModelCreating(modelBuilder);
            //EJEMPLOS DE SEMILLAS
            //modelBuilder.Entity<Rol>().HasData(
            //    new Rol { Id = 1, Nombre = "Administrador" },
            //    new Rol { Id = 2, Nombre = "Empleado" }
            //);

            //modelBuilder.Entity<Herramienta>().HasData(
            //    new Herramienta { Id = 1, Nombre = "Destornillador" },
            //    new Herramienta { Id = 2, Nombre = "Taladro" },
            //    new Herramienta { Id = 3, Nombre = "Martillo" }
            //);
        }
    }
}
