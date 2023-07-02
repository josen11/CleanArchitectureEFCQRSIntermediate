using CleanArchitecture.Domain.Common;

namespace CleanArchitecture.Domain
{
    public class Actor:BaseDomailModel
    {
        // Creamos el constructor para que se inicialice videos
        public Actor()
        {
            Videos = new HashSet<Video>();
        }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }

        // Relacion con videos M..M
        public virtual ICollection<Video> Videos { get; set;}
    }
}
