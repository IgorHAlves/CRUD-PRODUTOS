using System.ComponentModel.DataAnnotations;

namespace CRUD.PRODUTOS.APPLICATION.DTOs.Produto;

/// <summary>
/// Filtro e paginação da listagem de produtos. Os limites impedem que
/// um cliente peça uma página arbitrariamente grande.
/// </summary>
public class FiltroProdutoDTO
{
    public const int LimiteMaximo = 100;

    [StringLength(150)]
    public string? NomeProduto { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "A página deve ser maior ou igual a 1.")]
    public int Page { get; init; } = 1;

    [Range(1, LimiteMaximo, ErrorMessage = "O limite deve estar entre 1 e 100.")]
    public int Limit { get; init; } = 10;
}
