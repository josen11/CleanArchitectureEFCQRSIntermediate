using CleanArchitecture.Domain.Common;

namespace CleanArchitecture.Domain
{
    public class Video:BaseDomailModel
    {
        // Inicializamos Actores
        public Video()
        {
            Actores = new HashSet<Actor>();
        }
        public string? Nombre { get; set; }
        
        // Foreign key: Definimos la columna pero tambien una clase que sirva como hook de la entidad Foranea
        // Importante la nomenclatura que solicita EF es el nombre de la Clase que representa a la entidad Foreana + Id Eg. StreamerId
        public int StreamerId { get; set; }
        public virtual Streamer? Streamer { get; set; } // Virtual permitira que esta propiedad o methodo pueda ser sobrescritra por una entidad hijo

        // Relacion con Actores es M..M
        public virtual ICollection<Actor>? Actores { get; set; }

        // Relacion 1..1
        public virtual Director Director { get; set; }
    }
}
