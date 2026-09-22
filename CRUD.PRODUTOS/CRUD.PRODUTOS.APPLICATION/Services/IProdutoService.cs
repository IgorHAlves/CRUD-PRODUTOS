using CRUD.PRODUTOS.APPLICATION.Common;
using CRUD.PRODUTOS.APPLICATION.DTOs.Produto;

namespace CRUD.PRODUTOS.APPLICATION.Services;

public interface IProdutoService
{
    Task<int> CriarProdutoAsync(CriarProdutoDTO dto, CancellationToken cancellationToken = default);

    Task EditarProdutoAsync(int id, EditarProdutoDTO dto, CancellationToken cancellationToken = default);

    Task DeletarProdutoAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retorna o produto informado.
    /// </summary>
    /// <exception cref="DOMAIN.Exceptions.NaoEncontradoException">Quando o produto não existe.</exception>
    Task<VisualizarProdutoDTO> ListarProdutoAsync(int id, CancellationToken cancellationToken = default);

    Task<ResultadoPaginado<VisualizarProdutoDTO>> ListarProdutosAsync(
        FiltroProdutoDTO filtro,
        CancellationToken cancellationToken = default);
}
