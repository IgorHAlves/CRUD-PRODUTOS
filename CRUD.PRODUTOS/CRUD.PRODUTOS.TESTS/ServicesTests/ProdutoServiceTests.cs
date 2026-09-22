using CRUD.PRODUTOS.APPLICATION.DTOs.Produto;
using CRUD.PRODUTOS.APPLICATION.Services;
using CRUD.PRODUTOS.DOMAIN.Exceptions;
using CRUD.PRODUTOS.DOMAIN.Models;
using CRUD.PRODUTOS.DOMAIN.Repositories;
using CRUD.PRODUTOS.TESTS.Factories;
using Moq;
using Shouldly;
using Xunit;

namespace CRUD.PRODUTOS.TESTS.ServicesTests;

/// <summary>
/// Testa o serviço isolado do banco: o repositório e a unidade de trabalho
/// são dublês, de modo que uma falha aqui aponta sempre para a regra de negócio.
/// </summary>
public class ProdutoServiceTests
{
    private readonly Mock<IProdutoRepository> _produtoRepository = new(MockBehavior.Strict);
    private readonly Mock<IUnitOfWork> _unitOfWork = new(MockBehavior.Strict);
    private readonly IProdutoService _produtoService;

    public ProdutoServiceTests()
    {
        _unitOfWork
            .Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _produtoService = new ProdutoService(_produtoRepository.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Should_Criar_Produto()
    {
        //Arrange
        var dto = ProdutoBuilder.Criar();

        _produtoRepository
            .Setup(r => r.AdicionarAsync(It.IsAny<Produto>(), It.IsAny<CancellationToken>()))
            .Callback<Produto, CancellationToken>((p, _) => p.Id = 1)
            .Returns(Task.CompletedTask);

        //Act
        var idNovoProduto = await _produtoService.CriarProdutoAsync(dto);

        //Assert
        idNovoProduto.ShouldBe(1);
        _unitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_Criar_Produto_Removendo_Espacos_Do_Nome()
    {
        //Arrange
        Produto? adicionado = null;

        _produtoRepository
            .Setup(r => r.AdicionarAsync(It.IsAny<Produto>(), It.IsAny<CancellationToken>()))
            .Callback<Produto, CancellationToken>((p, _) => adicionado = p)
            .Returns(Task.CompletedTask);

        //Act
        await _produtoService.CriarProdutoAsync(ProdutoBuilder.Criar(nome: "  Camiseta  "));

        //Assert
        adicionado.ShouldNotBeNull();
        adicionado.Nome.ShouldBe("Camiseta");
    }

    [Fact]
    public async Task Should_Visualizar_Produto()
    {
        //Arrange
        var produto = ProdutoBuilder.Entidade(id: 7);

        _produtoRepository
            .Setup(r => r.ObterPorIdAsync(7, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(produto);

        //Act
        var visualizacao = await _produtoService.ListarProdutoAsync(7);

        //Assert
        visualizacao.Id.ShouldBe(produto.Id);
        visualizacao.Nome.ShouldBe(produto.Nome);
        visualizacao.Descricao.ShouldBe(produto.Descricao);
        visualizacao.Preco.ShouldBe(produto.Preco);
        visualizacao.QuantidadeEmEstoque.ShouldBe(produto.QuantidadeEmEstoque);
    }

    [Fact]
    public async Task Should_Throw_Visualizar_Produto_Id_Inexistente()
    {
        //Arrange
        _produtoRepository
            .Setup(r => r.ObterPorIdAsync(99, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Produto?)null);

        //Act
        var ex = await Should.ThrowAsync<NaoEncontradoException>(
            () => _produtoService.ListarProdutoAsync(99));

        //Assert
        ex.Message.ShouldBe("Produto 99 não encontrado");
    }

    [Fact]
    public async Task Should_Visualizar_Lista_Produtos()
    {
        //Arrange
        var produtos = new[]
        {
            ProdutoBuilder.Entidade(id: 1, nome: "Camiseta"),
            ProdutoBuilder.Entidade(id: 2, nome: "Calça")
        };

        _produtoRepository
            .Setup(r => r.BuscarAsync(null, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((produtos, produtos.Length));

        //Act
        var resultado = await _produtoService.ListarProdutosAsync(new FiltroProdutoDTO());

        //Assert
        resultado.Itens.Count.ShouldBe(2);
        resultado.TotalItens.ShouldBe(2);
        resultado.PaginaAtual.ShouldBe(1);
        resultado.TotalPaginas.ShouldBe(1);
    }

    [Fact]
    public async Task Should_Calcular_Total_De_Paginas()
    {
        //Arrange: 25 itens em páginas de 10 => 3 páginas
        var pagina = new[] { ProdutoBuilder.Entidade(id: 1) };

        _produtoRepository
            .Setup(r => r.BuscarAsync(null, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((pagina, 25));

        //Act
        var resultado = await _produtoService.ListarProdutosAsync(new FiltroProdutoDTO());

        //Assert
        resultado.TotalPaginas.ShouldBe(3);
        resultado.TotalItens.ShouldBe(25);
    }

    [Fact]
    public async Task Should_Visualizar_Lista_Produtos_Filtro_Nome()
    {
        //Arrange
        var filtro = new FiltroProdutoDTO { NomeProduto = "Camis", Page = 2, Limit = 5 };
        var encontrados = new[] { ProdutoBuilder.Entidade(id: 1, nome: "Camiseta") };

        _produtoRepository
            .Setup(r => r.BuscarAsync("Camis", 2, 5, It.IsAny<CancellationToken>()))
            .ReturnsAsync((encontrados, 1));

        //Act
        var resultado = await _produtoService.ListarProdutosAsync(filtro);

        //Assert: o filtro e a paginação chegam intactos ao repositório
        resultado.Itens.Single().Nome.ShouldBe("Camiseta");
        resultado.PaginaAtual.ShouldBe(2);
        _produtoRepository.Verify(
            r => r.BuscarAsync("Camis", 2, 5, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_Editar_Produto()
    {
        //Arrange
        var produto = ProdutoBuilder.Entidade(id: 3);
        var dto = ProdutoBuilder.Editar();

        _produtoRepository
            .Setup(r => r.ObterPorIdAsync(3, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(produto);

        //Act
        await _produtoService.EditarProdutoAsync(3, dto);

        //Assert
        produto.Nome.ShouldBe(dto.Nome);
        produto.Descricao.ShouldBe(dto.Descricao);
        produto.Preco.ShouldBe(dto.Preco);
        produto.QuantidadeEmEstoque.ShouldBe(dto.QuantidadeEmEstoque);
        _unitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_Throw_Editar_Produto_Nao_Encontrado()
    {
        //Arrange
        _produtoRepository
            .Setup(r => r.ObterPorIdAsync(99, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Produto?)null);

        //Act + Assert
        await Should.ThrowAsync<NaoEncontradoException>(
            () => _produtoService.EditarProdutoAsync(99, ProdutoBuilder.Editar()));

        _unitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Should_Deletar_Produto()
    {
        //Arrange
        var produto = ProdutoBuilder.Entidade(id: 4);

        _produtoRepository
            .Setup(r => r.ObterPorIdAsync(4, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(produto);
        _produtoRepository.Setup(r => r.Remover(produto));

        //Act
        await _produtoService.DeletarProdutoAsync(4);

        //Assert
        _produtoRepository.Verify(r => r.Remover(produto), Times.Once);
        _unitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_Throw_Deletar_Produto_Nao_Encontrado()
    {
        //Arrange
        _produtoRepository
            .Setup(r => r.ObterPorIdAsync(1, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Produto?)null);

        //Act + Assert
        await Should.ThrowAsync<NaoEncontradoException>(
            () => _produtoService.DeletarProdutoAsync(1));

        _unitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
