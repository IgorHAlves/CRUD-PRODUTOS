using CRUD.PRODUTOS.DATA.Data;
using CRUD.PRODUTOS.DOMAIN.Models;
using CRUD.PRODUTOS.INTERFACES;
using Microsoft.EntityFrameworkCore;

namespace CRUD.PRODUTOS.DATA.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDBContext _dbContext;

    public UsuarioRepository(AppDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Usuario?> ObterPorLoginAsync(string login)
    {
        return await _dbContext.Usuarios.FirstOrDefaultAsync(u => u.Login == login);
    }

    public async Task CriarAsync(Usuario usuario)
    {
        var existe = await _dbContext.Usuarios.AnyAsync(u => u.Login == usuario.Login);
        if (existe)
        {
            throw new ArgumentException("Login já cadastrado");
        }
        await _dbContext.Usuarios.AddAsync(usuario);
    }
}