namespace CRUD.PRODUTOS.APPLICATION.DTOs.Produto;

public class VisualizarProdutoDTO
{
    public required int Id { get; init; }
    public required string Nome { get; init; }
    public string? Descricao { get; init; }
    public required decimal Preco { get; init; }
    public required int QuantidadeEmEstoque { get; init; }
}
