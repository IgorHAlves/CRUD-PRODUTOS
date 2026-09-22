using System.ComponentModel.DataAnnotations;

namespace CRUD.PRODUTOS.APPLICATION.DTOs.Produto;

public class CriarProdutoDTO
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 150 caracteres.")]
    public string Nome { get; init; } = null!;

    [StringLength(1000, ErrorMessage = "A descrição deve ter no máximo 1000 caracteres.")]
    public string? Descricao { get; init; }

    [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "O preço deve ser maior que zero.")]
    public decimal Preco { get; init; }

    [Range(0, int.MaxValue, ErrorMessage = "A quantidade em estoque não pode ser negativa.")]
    public int QuantidadeEmEstoque { get; init; }
}
