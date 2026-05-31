using Microsoft.EntityFrameworkCore;
using API_AchadosEPerdidos.Features.Users.Models;
using API_AchadosEPerdidos.Features.Itens.Models;
using API_AchadosEPerdidos.Features.Local.Models;

namespace API_AchadosEPerdidos.Shared.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<AcessoUsuario> Acessos { get; set; }
    public DbSet<Item> Itens { get; set; }
    public DbSet<ItemImage> ItemImages { get; set; }
    public DbSet<API_AchadosEPerdidos.Features.Local.Models.Local> Locais { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<StatusItem> StatusItens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>().ToTable("usuario");
        modelBuilder.Entity<AcessoUsuario>().ToTable("acesso_usuario");
        modelBuilder.Entity<Item>().ToTable("item");
        modelBuilder.Entity<ItemImage>().ToTable("item_images");
        modelBuilder.Entity<API_AchadosEPerdidos.Features.Local.Models.Local>().ToTable("local");
        modelBuilder.Entity<Categoria>().ToTable("categoria");
        modelBuilder.Entity<StatusItem>().ToTable("status_item");
    }
}