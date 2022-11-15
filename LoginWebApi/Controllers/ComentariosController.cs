using AutoMapper;
using LoginWebApi.DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoginWebApi.Controllers;

[ApiController]
[Route("api/comentarios/{libroId:int}")]
public class ComentariosController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<IdentityUser> _userManager;
    public ComentariosController(ApplicationDbContext context, IMapper mapper, UserManager<IdentityUser> userManager )
    { 
        this._context = context;
        this._mapper = mapper;
        this._userManager = userManager;
    }


    [HttpGet]
    public async Task<ActionResult<List<ComentariosDTOs>>> GetAllComment(int libroId)
    {
        
        var existeLibro = await _context.libros.AnyAsync(x => x.Id == libroId);

        if (!existeLibro)
        {
            return NotFound();
        }
        
        var getComments = await _context.Comentarios.Where(x => x.LibroId == libroId).ToListAsync();

        return  _mapper.Map<List<ComentariosDTOs>>(getComments);
    }

    [HttpGet("{id:int}", Name = "ObtenerComentario")]
    public async Task<ActionResult<ComentariosDTOs>> GetById(int id)
    {
        var comentarios = await _context.Comentarios.FirstOrDefaultAsync(x => x.Id == id);

        if (comentarios == null)
        {
            return NotFound();
        }

        return _mapper.Map<ComentariosDTOs>(comentarios);
    }


    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateComment(ComentariosDTOs comentario, int id, int libroId)
    {
        var existComment = await _context.Comentarios.AnyAsync(x => x.Id == id);

        if (!existComment)
        {
            return NotFound();
        }

        var updateComment = _mapper.Map<Comentarios>(comentario);
        updateComment.Id = id;
        updateComment.LibroId = libroId;
        _context.Update(updateComment);

        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult> Addcomment(int libroId, [FromBody] ComentariosDTOs comentarios)
    {

        var emailClaims = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
        var email = emailClaims.Value;
        var usuario = await _userManager.FindByEmailAsync(email);
        var usuarioId = usuario.Id;
        var existeLibro = await _context.libros.AnyAsync(x => x.Id == libroId);

        if (!existeLibro)
        {
            return NotFound();
        }
        var addComment = _mapper.Map<Comentarios>(comentarios);
        addComment.LibroId = libroId;
        addComment.UsuarioId = usuarioId;
        _context.Add(addComment);

        await _context.SaveChangesAsync();

        var comentarioDto = _mapper.Map<ComentariosDTOs>(addComment);

        return CreatedAtRoute("ObtenerComentario", new { id = addComment.Id, libroId = addComment.LibroId }, comentarioDto);
    }
    
    
    
  
}