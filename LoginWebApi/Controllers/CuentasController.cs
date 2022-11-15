using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LoginWebApi.DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace LoginWebApi.Controllers;

[ApiController]
[Route("api/controller")]
public class CuentasController : ControllerBase
{

    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IDataProtector _dataProtector;
    public CuentasController(UserManager<IdentityUser> userManager, IConfiguration configuration, SignInManager<IdentityUser> signInManager, IDataProtectionProvider dataProtectionProvider)
    {
        this._userManager = userManager;
        this._configuration = configuration;
        this._signInManager = signInManager;
        _dataProtector = dataProtectionProvider.CreateProtector("secreto_de_no_saber");
    }


    [HttpGet("encriptar")]
    public ActionResult Encriptar()
    {
        var textoPlano = "Anyelo Vinzen";
        var textoCifrado = _dataProtector.Protect(textoPlano);
        var textoDesencriptado = _dataProtector.Unprotect(textoCifrado);


        return Ok(new
            {
                textoPlano = textoPlano,
                textoCifrado = textoCifrado,
                textoDesencriptado = textoDesencriptado
            });

    }
    
    [HttpGet("encriptarPorTiempo")]
    public ActionResult EncriptarPorTiempo()
    {

        var protectorLimitadorPorTiempo = _dataProtector.ToTimeLimitedDataProtector();
        var textoPlano = "Anyelo Vinzen";
        var textoCifrado = protectorLimitadorPorTiempo.Protect(textoPlano,lifetime:TimeSpan.FromSeconds(5));
       // Thread.Sleep(6000);
        var textoDesencriptado = protectorLimitadorPorTiempo.Unprotect(textoCifrado);


        return Ok(new
        {
            textoPlano = textoPlano,
            textoCifrado = textoCifrado,
            textoDesencriptado = textoDesencriptado
        });

    }

    [HttpPost("registrar")]
    public async Task<ActionResult<RespuestaAutenticacion>> LoginUserRegistrar(CredencialesUsuario credencialesUsuario)
    {
        var usuario = new IdentityUser{UserName = credencialesUsuario.Email, Email = credencialesUsuario.Email};
        var resultado = await _userManager.CreateAsync(usuario, credencialesUsuario.Password);

        if (resultado.Succeeded)
        {
            return await construirTokens(credencialesUsuario);
        }
        else
        {
            return BadRequest(resultado.Errors);
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<RespuestaAutenticacion>> Login(CredencialesUsuario credencialesUsuario)
    {

        var resultado =
            await _signInManager.PasswordSignInAsync(credencialesUsuario.Email, credencialesUsuario.Password, false,
                false);
        if (resultado.Succeeded)
        {
            return await construirTokens(credencialesUsuario);
        }
        else
        {
            return BadRequest("Login incorrecto");
        }
    }

    [HttpGet]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<RespuestaAutenticacion>> Renovar()
    {

        var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
        var email = emailClaim.Value;

        var credencialesUsuario = new CredencialesUsuario()
        {
            Email = email
        };


        return await construirTokens(credencialesUsuario);
    }

    private async Task<RespuestaAutenticacion> construirTokens(CredencialesUsuario credencialesUsuario)
    {
        var claims = new List<Claim>()
        {
            new Claim("email", credencialesUsuario.Email)
        };

        var usuario = await _userManager.FindByEmailAsync(credencialesUsuario.Email);
        //Obtenemos todos los permisos del usuario de la base de datos.
        var claimsDB = await _userManager.GetClaimsAsync(usuario);
        
        claims.AddRange(claimsDB);
        
        var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["llavejwt"]));

        var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);

        var expiracion = DateTime.UtcNow.AddYears(30);

        var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires:expiracion, signingCredentials:creds);

        return new RespuestaAutenticacion()
        {
            Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
            Expiracion = expiracion
        };

    }

    [HttpPost("HacerAdmin")]
    public async Task<ActionResult> HacerAdmin(EditarAdminDTO editarAdminDto)
    {
        var usuario = await _userManager.FindByEmailAsync(editarAdminDto.Email);
        await _userManager.AddClaimAsync(usuario, new Claim("esAdmin","1"));

        return NoContent();
    }
    
    
    [HttpPost("RemoverAdmin")]
    public async Task<ActionResult> RemoverAdmin(EditarAdminDTO editarAdminDto)
    {
        var usuario = await _userManager.FindByEmailAsync(editarAdminDto.Email);
        await _userManager.RemoveClaimAsync(usuario, new Claim("esAdmin","1"));

        return NoContent();
    }
}