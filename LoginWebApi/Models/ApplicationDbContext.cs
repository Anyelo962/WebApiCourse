using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LoginWebApi;

public class ApplicationDbContext :IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<AutorLibro>()
            .HasKey(al => new { al.AutorId, al.LibroId });
    }

    public DbSet<Autor> Autors { get; set; }
    public DbSet<Libro> libros { get; set; }
    public DbSet<Comentarios> Comentarios { get; set; }
    public DbSet<AutorLibro> AutorLibros { get; set; }

}