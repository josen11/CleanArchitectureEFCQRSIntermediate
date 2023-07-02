using CleanArchitecture.Domain.Common;

namespace CleanArchitecture.Domain
{
    public class Director:BaseDomailModel
    {


        public string? Nombre { get; set; }
        public string? Apellido { get; set; }

        // Relation 1..1
        public int VideoId { get; set; }
        public virtual Video? Video { get; set; }
    }
}
