using CRUD.PRODUTOS.APPLICATION.DTOs.Produto;
using CRUD.PRODUTOS.DOMAIN.Models;

namespace CRUD.PRODUTOS.APPLICATION.Mappings;

/// <summary>
/// Conversões entre a entidade Produto e seus DTOs, centralizadas para
/// não se repetirem em cada serviço.
/// </summary>
public static class ProdutoMappings
{
    public static VisualizarProdutoDTO ParaVisualizacao(this Produto produto) =>
        new()
        {
            Id = produto.Id,
            Nome = produto.Nome,
            Descricao = produto.Descricao,
            Preco = produto.Preco,
            QuantidadeEmEstoque = produto.QuantidadeEmEstoque
        };

    public static IReadOnlyList<VisualizarProdutoDTO> ParaVisualizacao(this IEnumerable<Produto> produtos) =>
        produtos.Select(ParaVisualizacao).ToList();

    public static Produto ParaEntidade(this CriarProdutoDTO dto) =>
        new()
        {
            Nome = dto.Nome.Trim(),
            Descricao = dto.Descricao?.Trim(),
            Preco = dto.Preco,
            QuantidadeEmEstoque = dto.QuantidadeEmEstoque
        };

    public static void AplicarEm(this EditarProdutoDTO dto, Produto produto)
    {
        produto.Nome = dto.Nome.Trim();
        produto.Descricao = dto.Descricao?.Trim();
        produto.Preco = dto.Preco;
        produto.QuantidadeEmEstoque = dto.QuantidadeEmEstoque;
    }
}
