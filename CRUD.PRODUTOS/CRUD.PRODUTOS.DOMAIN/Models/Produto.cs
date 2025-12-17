namespace CRUD.PRODUTOS.DOMAIN.Models;

public class Produto : EntityBase
{
    public string Nome { get; set; }
    public string? Descricao { get; set; }
    public decimal Preco { get; set; }
    public int QuantidadeEmEstoque { get; set; }
}