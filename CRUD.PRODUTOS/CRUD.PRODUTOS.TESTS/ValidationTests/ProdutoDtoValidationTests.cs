using System.ComponentModel.DataAnnotations;
using CRUD.PRODUTOS.APPLICATION.DTOs.Produto;
using CRUD.PRODUTOS.TESTS.Factories;
using Shouldly;
using Xunit;

namespace CRUD.PRODUTOS.TESTS.ValidationTests;

/// <summary>
/// As regras de formato saíram do serviço e passaram a ser declarativas nos
/// DTOs — o [ApiController] as aplica antes de a action executar. Estes testes
/// cobrem o que antes era verificado com ifs dentro do ProdutoService.
/// </summary>
public class ProdutoDtoValidationTests
{
    private static IReadOnlyList<ValidationResult> Validar(object dto)
    {
        var resultados = new List<ValidationResult>();

        Validator.TryValidateObject(dto, new ValidationContext(dto), resultados, validateAllProperties: true);

        return resultados;
    }

    [Theory]
    [InlineData(-80)]
    [InlineData(0)]
    public void Should_Rejeitar_Criar_Produto_Com_Preco_Invalido(decimal preco)
    {
        var erros = Validar(ProdutoBuilder.Criar(preco: preco));

        erros.ShouldContain(e => e.ErrorMessage == "O preço deve ser maior que zero.");
    }

    [Fact]
    public void Should_Rejeitar_Criar_Produto_Com_Quantidade_Negativa()
    {
        var erros = Validar(ProdutoBuilder.Criar(quantidade: -1));

        erros.ShouldContain(e => e.ErrorMessage == "A quantidade em estoque não pode ser negativa.");
    }

    [Fact]
    public void Should_Rejeitar_Criar_Produto_Sem_Nome()
    {
        var erros = Validar(ProdutoBuilder.Criar(nome: ""));

        erros.ShouldNotBeEmpty();
    }

    [Fact]
    public void Should_Aceitar_Criar_Produto_Valido()
    {
        Validar(ProdutoBuilder.Criar()).ShouldBeEmpty();
    }

    [Theory]
    [InlineData(-80)]
    [InlineData(0)]
    public void Should_Rejeitar_Editar_Produto_Com_Preco_Invalido(decimal preco)
    {
        // Regra alinhada com a criação: antes, editar aceitava preço zero.
        var erros = Validar(ProdutoBuilder.Editar(preco: preco));

        erros.ShouldContain(e => e.ErrorMessage == "O preço deve ser maior que zero.");
    }

    [Fact]
    public void Should_Rejeitar_Editar_Produto_Com_Quantidade_Negativa()
    {
        var erros = Validar(ProdutoBuilder.Editar(quantidade: -1));

        erros.ShouldContain(e => e.ErrorMessage == "A quantidade em estoque não pode ser negativa.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_Rejeitar_Pagina_Invalida(int page)
    {
        var erros = Validar(new FiltroProdutoDTO { Page = page });

        erros.ShouldContain(e => e.ErrorMessage == "A página deve ser maior ou igual a 1.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(FiltroProdutoDTO.LimiteMaximo + 1)]
    public void Should_Rejeitar_Limite_Fora_Do_Intervalo(int limit)
    {
        // Protege contra o limite zero (divisão por zero no cálculo de páginas)
        // e contra páginas arbitrariamente grandes.
        var erros = Validar(new FiltroProdutoDTO { Limit = limit });

        erros.ShouldContain(e => e.ErrorMessage == "O limite deve estar entre 1 e 100.");
    }

    [Fact]
    public void Should_Usar_Paginacao_Padrao()
    {
        var filtro = new FiltroProdutoDTO();

        filtro.Page.ShouldBe(1);
        filtro.Limit.ShouldBe(10);
        Validar(filtro).ShouldBeEmpty();
    }
}
