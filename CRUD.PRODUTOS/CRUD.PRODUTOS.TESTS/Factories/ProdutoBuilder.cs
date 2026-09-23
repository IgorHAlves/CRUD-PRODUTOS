using CRUD.PRODUTOS.APPLICATION.DTOs.Produto;
using CRUD.PRODUTOS.DOMAIN.Models;

namespace CRUD.PRODUTOS.TESTS.Factories;

/// <summary>
/// Dados de teste com valores padrão válidos, para que cada teste declare
/// apenas o que é relevante para ele.
/// </summary>
public static class ProdutoBuilder
{
    public static Produto Entidade(
        int id = 0,
        string nome = "Camiseta",
        string? descricao = "Camiseta de algodão",
        decimal preco = 80m,
        int quantidade = 20) =>
        new()
        {
            Id = id,
            Nome = nome,
            Descricao = descricao,
            Preco = preco,
            QuantidadeEmEstoque = quantidade
        };

    public static CriarProdutoDTO Criar(
        string nome = "Camiseta",
        string? descricao = "Camiseta de algodão",
        decimal preco = 80m,
        int quantidade = 20) =>
        new()
        {
            Nome = nome,
            Descricao = descricao,
            Preco = preco,
            QuantidadeEmEstoque = quantidade
        };

    public static EditarProdutoDTO Editar(
        string nome = "Camiseta Editada",
        string? descricao = "Nova descrição",
        decimal preco = 50m,
        int quantidade = 10) =>
        new()
        {
            Nome = nome,
            Descricao = descricao,
            Preco = preco,
            QuantidadeEmEstoque = quantidade
        };
}
