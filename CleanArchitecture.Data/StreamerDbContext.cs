using CleanArchitecture.Domain;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Data
{
    public class StreamerDbContext:DbContext
    {
        // Create conexion string
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Podemos usar \\ o usar @ verbatim para que se lea el \ como valido dentro del string.
            // Data Source: Server y nombre de la instancia de SQL Server
            // Initial Catalog: BD
            // Integrated Security = true: Authentication mediante Windows.
            optionsBuilder.UseSqlServer("Data Source=DESKTOP-CGLPR6C\\SQLSERVERDEV;Initial Catalog=Streamer;Integrated Security=True;TrustServerCertificate=True;")
            // Logs de los comandos que se estan utilizando en Background cuando utilizando en EF y que se impriman en la Consola
            .LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name},Microsoft.Extensions.Logging.LogLevel.Information)
            // Metodo nos permite describir cada una de las operaciones
            .EnableSensitiveDataLogging();
        }

        // Definir Fluent API
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Fluent API: Delacion de uno a muchos para Streamer Class: Relationship, Foreign key column, Is required and Delete Behavior
            // Importante tener en consideracion que FluenAPI es opcional siempre y cuando se siga las convenciones de EF para el nombrado de columnas para relaciones 1 a muchos
            // Debemos usar FluenAPI cuando definimos los atributos que son las claves foraneas sin seguir la convencion que recomienda EF
            modelBuilder.Entity<Streamer>()
                 .HasMany(m => m.Videos)
                 .WithOne(m => m.Streamer)
                 .HasForeignKey(m => m.StreamerId)
                 .IsRequired()
                 .OnDelete(DeleteBehavior.Restrict);

            // Definir el M..M entre cliente y video
            modelBuilder.Entity<Video>()
                .HasMany(p => p.Actores)
                .WithMany(t => t.Videos)
                .UsingEntity<VideoActor>(
                    pt => pt.HasKey(e => new { e.ActorId, e.VideoId})
                );
        }



        // Defining entities
        public DbSet<Streamer>? Streamers { get; set; }
        public DbSet<Video>? Videos { get; set; }
        public DbSet<Actor>? Actores { get; set; }
        public DbSet<Director>? Directores { get; set; }
    }
}
