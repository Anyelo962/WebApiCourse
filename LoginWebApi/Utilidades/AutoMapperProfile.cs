using AutoMapper;
using LoginWebApi.DTOs;

namespace LoginWebApi.Utilidades;

public class AutoMapperProfile:Profile
{
    public AutoMapperProfile()
    {
        CreateMap<AutorDTOs, Autor>();
        CreateMap<Autor, AutorDTOs>();
        CreateMap<LibroPatchDTOs, Libro>().ReverseMap();
        CreateMap<Autor, AutorDTOConLibro>()
            .ForMember(autorDto => autorDto.librosDto, opciones => opciones.MapFrom(MapAutorDtoLibros));
        CreateMap<LibrosDTOs, Libro>();
        CreateMap<LibroDTOConAutor, Libro>()
            .ForMember(libro => libro.AutoresLibrosList, opciones => opciones.MapFrom(MapAutoresLibro));
        CreateMap<Libro, LibrosDTOs>()
            .ForMember(libroDto => libroDto.Autores, opciones => opciones.MapFrom(mapLibrosDtoAutores));
        CreateMap<ComentariosDTOs, Comentarios>().ReverseMap();
    }


    private List<LibrosDTOs> MapAutorDtoLibros(Autor autor, AutorDTOs autorDtOs)
    {
        var resultado = new List<LibrosDTOs>();

        if (resultado.Count == null)
        {
            return resultado;
        }
        
        foreach (var autores in autor.AutoresLibrosList)
        {
            resultado.Add(new LibrosDTOs()
            {
                Id = autores.Libro.Id,
                Titulo = autores.Libro.Titulo
            });
        }

        return resultado;
    }

    private List<AutorDTOs> mapLibrosDtoAutores(Libro libro, LibrosDTOs librosDtOs)
    {
        var resultado = new List<AutorDTOs>();

        if (libro.AutoresLibrosList == null)
        {
            return resultado;
        }

        foreach (var autorLibro in libro.AutoresLibrosList)
        {
            resultado.Add(new AutorDTOs()
            {
                Id = autorLibro.AutorId,
                nombre = autorLibro.Autor.nombre
            });
        }

        return resultado;
    }
    private List<AutorLibro> MapAutoresLibro(LibrosDTOs librosDtos, Libro libro)
    {
        var resultado = new List<AutorLibro>();

        if (librosDtos.autoresIds == null) return resultado;


        foreach (var autorId in librosDtos.autoresIds)
        {
            resultado.Add(new AutorLibro() {AutorId = autorId});
            
        }

        return resultado;
    }
    
}