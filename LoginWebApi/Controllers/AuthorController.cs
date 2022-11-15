using AutoMapper;
using LoginWebApi.DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace LoginWebApi.Controllers;

[ApiController]
[Route("api/[Controller]")]
public class AuthorController : ControllerBase
{
    private readonly ApplicationDbContext context;
    private readonly IMapper _mapper;

    public AuthorController(ApplicationDbContext context, IMapper mapper)
    {
        this.context = context;
        this._mapper = mapper;
    }

    [HttpGet]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
    public async Task<List<AutorDTOs>> GetAllAuthor()
    {
        var getAll = await context.Autors.ToListAsync();

        return _mapper.Map<List<AutorDTOs>>(getAll);
    }

    [HttpGet("{id:int}", Name = "obtenerAutor")]
    public async Task<ActionResult<AutorDTOs>> GetById(int id)
    {
        var getAuthorById = await context.Autors
            .Include(x => x.AutoresLibrosList)
            .ThenInclude(x => x.Libro)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (string.IsNullOrEmpty(getAuthorById.ToString()))
        {
            return NotFound();
        }

        return Ok(_mapper.Map<AutorDTOs>(getAuthorById));
    }

    [HttpGet("{nombre}")]
    public async Task<ActionResult<List<AutorDTOs>>> GetByName([FromRoute] string nombre)
    {
        var getAuthorByName = await context.Autors.Where(x => x.nombre.Contains(nombre)).ToListAsync();

        return _mapper.Map<List<AutorDTOs>>(getAuthorByName);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateAuthor(AutorDTOs autorDtOs, int id)
    {
        var exist = await context.Autors.AnyAsync(x => x.Id == id);

        if (!exist)
        {
            return NotFound();
        }

        var autorDto = _mapper.Map<Autor>(autorDtOs);
        autorDto.Id = id;

        context.Update(autorDto);
        await context.SaveChangesAsync();

        return NoContent();
    }
    
    [HttpPost]
    public async Task<ActionResult> AddAuthor([FromBody] AutorDTOs autorDto)
    {
        var autorMapper = _mapper.Map<Autor>(autorDto);

        context.Autors.Add(autorMapper);

        await context.SaveChangesAsync();

        var autorDTO = _mapper.Map<AutorDTOs>(autorMapper);

        return CreatedAtRoute("obtenerAutor", new { id = autorMapper.Id }, autorDto);
    }

    [HttpPatch]
    public async Task<ActionResult> PatchAutor(int id, JsonPatchDocument<AutorPatchDTO> patchAutor)
    {
        return Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> delete(int id)
    {
        var existe = await context.Autors.AnyAsync(x => x.Id == id);

        if (!existe)
        {
            return NotFound();
        }
        context.Autors.Remove(new Autor(){Id = id});
        await context.SaveChangesAsync();

        return Ok("Autor eliminado");
    }
}