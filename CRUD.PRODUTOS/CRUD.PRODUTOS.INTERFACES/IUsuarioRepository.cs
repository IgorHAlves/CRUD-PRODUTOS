using CRUD.PRODUTOS.DOMAIN.Models;

namespace CRUD.PRODUTOS.INTERFACES;

public interface IUsuarioRepository
{
    Task<Usuario?> ObterPorLoginAsync(string login);
    Task CriarAsync(Usuario usuario);
}
