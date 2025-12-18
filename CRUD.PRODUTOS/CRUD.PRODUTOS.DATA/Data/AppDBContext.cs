using CRUD.PRODUTOS.DOMAIN;
using CRUD.PRODUTOS.DOMAIN.Models;
using Microsoft.EntityFrameworkCore;

namespace CRUD.PRODUTOS.DATA.Data;

public class AppDBContext : DbContext  
{
    public AppDBContext(DbContextOptions<AppDBContext> options) :  base(options)
    {
    }
    public DbSet<Produto> Produtos { get; set; }
    
    public DbSet<Usuario> Usuarios { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Produto>()
            .HasKey(k => k.Id);

        
        modelBuilder.Entity<Usuario>()
            .HasKey(k => k.Id);
        
        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.Login)
            .IsUnique();



    }
}