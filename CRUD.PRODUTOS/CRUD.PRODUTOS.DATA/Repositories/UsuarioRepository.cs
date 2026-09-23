using CRUD.PRODUTOS.DATA.Data;
using CRUD.PRODUTOS.DOMAIN.Models;
using CRUD.PRODUTOS.DOMAIN.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CRUD.PRODUTOS.DATA.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDBContext _dbContext;

    public UsuarioRepository(AppDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Usuario?> ObterPorLoginAsync(string login, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Login == login, cancellationToken);
    }

    public async Task<Usuario?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Usuarios.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<bool> ExisteLoginAsync(string login, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Usuarios.AnyAsync(u => u.Login == login, cancellationToken);
    }

    public Task AdicionarAsync(Usuario usuario, CancellationToken cancellationToken = default)
    {
        _dbContext.Usuarios.Add(usuario);

        return Task.CompletedTask;
    }
}
