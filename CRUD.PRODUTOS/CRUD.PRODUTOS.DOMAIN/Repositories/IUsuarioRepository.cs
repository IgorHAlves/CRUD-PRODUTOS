using CRUD.PRODUTOS.DOMAIN.Models;

namespace CRUD.PRODUTOS.DOMAIN.Repositories;

public interface IUsuarioRepository
{
    Task<Usuario?> ObterPorLoginAsync(string login, CancellationToken cancellationToken = default);

    Task<Usuario?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> ExisteLoginAsync(string login, CancellationToken cancellationToken = default);

    Task AdicionarAsync(Usuario usuario, CancellationToken cancellationToken = default);
}
