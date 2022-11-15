using System.ComponentModel.DataAnnotations;

namespace LoginWebApi;

public class Autor
{
    public int Id { get; set; }
    [StringLength(maximumLength:120)]
    public string nombre { get; set; }

    public DateTime? fechaPublicacion { get; set; }
    public List<AutorLibro> AutoresLibrosList { get; set; }

}