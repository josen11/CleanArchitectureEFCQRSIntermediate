using CleanArchitecture.Domain.Common;

namespace CleanArchitecture.Domain
{
    public class Streamer:BaseDomailModel
    {
        // Note
        // Fluent API permite hacer el codigo mas legible y es considerado una buena practica.
        public string? Nombre { get; set; }
        public string? Url { get; set; }

        // A nivel de la clase Foreana, debemos representar en una List<> las clase secundaria (es decir el 1 .. *)
        // Tambien podemos usar un ICollection<> en vez de List<>, esto se aplica mas cuando queremos que asociar el attributo a la abstranccion y que esta nos permita implementar un List<> o HashTable<>
        // o cualquier otra tipo de collecion a la entidad secundaria 
        public ICollection<Video>? Videos { get; set; }
    }
}
