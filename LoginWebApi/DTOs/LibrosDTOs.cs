namespace LoginWebApi.DTOs;

public class LibrosDTOs
{
    public int Id { get; set; }
    public string Titulo { get; set; }
    public List<AutorDTOs> Autores { get; set; }
    public DateTime fechaPublicacion { get; set; }
    public List<int> autoresIds { get; set; }
    public List<AutorLibro> AutoresLibrosList { get; set; }
}