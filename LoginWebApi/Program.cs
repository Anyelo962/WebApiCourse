using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using LoginWebApi;
using LoginWebApi.Controllers;
using LoginWebApi.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.


JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
builder.Services.AddControllers(opciones =>
{
    opciones.Filters.Add(typeof(FiltroDeExcepcion));
}).AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles).AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWK",
        In = ParameterLocation.Header
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]
            {
            
            }
        }
    });
});

builder.Services.AddTransient<AuthorController>();
builder.Services.AddTransient<LibrosController>();
builder.Services.AddTransient<Comentarios>();
builder.Services.AddTransient<CuentasController>();
// JsonSerializerOptions options = new()
// {
//     ReferenceHandler = ReferenceHandler.IgnoreCycles,
//     WriteIndented = true
// };

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddAuthorization(option =>
{
    option.AddPolicy("EsAdmin", politica=> politica.RequireClaim("esAdmin"));
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(build =>
    {
        build.WithOrigins("http://apirequest.io").AllowAnyMethod().AllowAnyHeader();
    });
});
builder.Services.AddDataProtection();

//Config identity for users and rols
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("defaultConnection")));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).
    AddJwtBearer(option => option.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["llavejwt"])),
        ClockSkew = TimeSpan.Zero
    });

//builder.Services.AddResponseCaching();
//builder.Services.AddTransient<MifiltroDeAccion>();
//builder.Services.AddHostedService<EscribirEnArchivo>();
var app = builder.Build();


// app.Use(async (Contexto, siguiente) =>
// {
//     using (var ms = new MemoryStream())
//     {
//         var cuerpoOriginalRespuesta = Contexto.Response.Body;
//         Contexto.Response.Body = ms;
//
//         await siguiente.Invoke();
//
//         ms.Seek(0, SeekOrigin.Begin);
//         string respuesta = new StreamReader(ms).ReadToEnd();
//
//         ms.Seek(0, SeekOrigin.Begin);
//
//         await ms.CopyToAsync(cuerpoOriginalRespuesta);
//         Contexto.Response.Body = cuerpoOriginalRespuesta;
//
//         
//     }
// });

// app.Map("/ruta1",
//     app => { app.Run(async contexto => { await contexto.Response.WriteAsync("Estoy interceptando la tuberia"); }); });

//app.UseMiddleware<LoguearRespuestaHttpMiddleware>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//app.UseLoguearRespuesta();
app.UseHttpsRedirection();
//app.UseResponseCaching();
app.UseCors();
app.UseAuthorization();

app.MapControllers();

app.Run();