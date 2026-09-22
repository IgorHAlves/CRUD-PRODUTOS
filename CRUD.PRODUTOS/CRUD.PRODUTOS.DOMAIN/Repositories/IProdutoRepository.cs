using CRUD.PRODUTOS.DOMAIN.Models;

namespace CRUD.PRODUTOS.DOMAIN.Repositories;

public interface IProdutoRepository
{
    Task AdicionarAsync(Produto produto, CancellationToken cancellationToken = default);

    /// <param name="rastrear">
    /// <c>true</c> quando a entidade será alterada e precisa ser acompanhada pelo
    /// change tracker; <c>false</c> (padrão) para consultas somente leitura.
    /// </param>
    Task<Produto?> ObterPorIdAsync(int id, bool rastrear = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retorna a página solicitada e o total de itens que satisfazem o filtro.
    /// A montagem do resultado paginado é responsabilidade da camada de aplicação.
    /// </summary>
    Task<(IReadOnlyList<Produto> Itens, int TotalItens)> BuscarAsync(
        string? nomeProduto,
        int page,
        int limit,
        CancellationToken cancellationToken = default);

    void Remover(Produto produto);
}
