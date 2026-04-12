using Microsoft.EntityFrameworkCore;
using API_AchadosEPerdidos.Features.Users.Models;

namespace API_AchadosEPerdidos.Shared.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<AcessoUsuario> Acessos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>().ToTable("usuario");
        modelBuilder.Entity<AcessoUsuario>().ToTable("acesso_usuario");
    }
}