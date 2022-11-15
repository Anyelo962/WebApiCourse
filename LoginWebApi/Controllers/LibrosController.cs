using AutoMapper;
using LoginWebApi.DTOs;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoginWebApi.Controllers;

[ApiController]
[Route("[Controller]")]
public class LibrosController : ControllerBase
{
    private readonly ApplicationDbContext context;
    private readonly IMapper _mapper;
    public LibrosController(ApplicationDbContext context, IMapper mapper)
    {
        this.context = context;
        this._mapper = mapper;
    }
    [HttpGet]
    public async Task<List<LibrosDTOs>> GetAllBook()
    {
        var getAllBook = await context.libros.ToListAsync();

        return _mapper.Map<List<LibrosDTOs>>(getAllBook);
    }
    
    [HttpGet("{id:int}")]
    public async Task<ActionResult<LibroDTOConAutor>> GetByIdBook(int id)
    {
        var getBook = await context.libros
            .Include(x => x.AutoresLibrosList)
            .ThenInclude(x => x.Autor)
            .FirstOrDefaultAsync(x => x.Id == id);

        getBook.AutoresLibrosList = getBook.AutoresLibrosList.OrderBy(x => x.Orden).ToList();

        return _mapper.Map<LibroDTOConAutor>(getBook);
    }

    [HttpGet("{nombre}")]
    public async Task<LibrosDTOs> GetByName([FromQuery] string nombre)
    {
        var getBookByName = await context.libros.Where(x => x.Titulo.Contains(nombre)).ToListAsync();

        return _mapper.Map<LibrosDTOs>(getBookByName);
        
    }

    [HttpPost]
    public async Task<ActionResult> AddBook( [FromBody] LibrosDTOs book)
    {

        if (book.autoresIds == null)
        {
            return BadRequest("No se puede crear libro sin autores!");
        }
        
        var autoresIds = await context.Autors.Where(x => book.autoresIds.Contains(x.Id)).Select(x => x.Id).ToListAsync();

        if (book.autoresIds.Count != autoresIds.Count)
        {
            return BadRequest("No existe el que esta buscando!");
        }

        var getBook = _mapper.Map<Libro>(book);
        AsignarOrdenAutores(getBook);
        context.libros.Add(getBook);
        await context.SaveChangesAsync();

        var libroDTO = _mapper.Map<LibrosDTOs>(getBook);
        return Ok("Se ha agregado el nuevo libro!");
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateBook(int id,LibrosDTOs librosDtOs)
    {
        var libroDB = await context.libros.Include(x => x.AutoresLibrosList)
            .FirstOrDefaultAsync(x => x.Id == id);
    
        if (libroDB is null)
        {
            return NotFound();
        }
    
        var update =  _mapper.Map(librosDtOs, libroDB);
        AsignarOrdenAutores(update);
    
        await context.SaveChangesAsync();
    
        return NoContent();
    }

    private void AsignarOrdenAutores(Libro libro)
    {
        if (libro.AutoresLibrosList != null)
        {
            for (int i = 0; i < libro.AutoresLibrosList.Count; i++)
            {
                libro.AutoresLibrosList[i].Orden = i;
            }
        }
    }
    
    [HttpPatch("{id:int}")]
    public async Task<ActionResult> UpdatePatch([FromBody] JsonPatchDocument<LibroPatchDTOs> patchDocument, int id)
    {

        if (patchDocument == null)
        {
            return BadRequest();
        }

        var libroDB = await context.Autors.FirstOrDefaultAsync(x => x.Id == id);

        if (libroDB is null)
        {
            return NotFound();
        }

        var libroDTO = _mapper.Map<LibroPatchDTOs>(libroDB);
        
        
        patchDocument.ApplyTo(libroDTO, ModelState);

        var esValido = TryValidateModel(libroDTO);

        if (!esValido)
        {
            return BadRequest(ModelState);
        }

        _mapper.Map(libroDTO, libroDB);

        await context.SaveChangesAsync();

        return NoContent();
    }
}