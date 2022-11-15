using System.ComponentModel.DataAnnotations;

namespace LoginWebApi;

public class Libro
{
    public int Id { get; set; }
    [Required]
    [StringLength(maximumLength:100)]
    public string Titulo { get; set; }
    //public List<Comentarios> Comentarios { get; set; }
    
    public DateTime? fechaPublicacionLibro { get; set; }
    public List<AutorLibro> AutoresLibrosList { get; set; }
}