using System.Reflection;
using CRUD.PRODUTOS.DOMAIN.Models;
using Microsoft.EntityFrameworkCore;

namespace CRUD.PRODUTOS.DATA.Data;

public class AppDBContext : DbContext
{
    public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
    {
    }

    public DbSet<Produto> Produtos => Set<Produto>();

    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
