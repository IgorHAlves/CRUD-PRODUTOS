namespace CRUD.PRODUTOS.DOMAIN.DTOs;

public class CriarProdutoDTO
{
    public string Nome { get; set; }
    public string? Descricao { get; set; }
    public decimal Preco { get; set; }
    public int QuantidadeEmEstoque { get; set; }
}