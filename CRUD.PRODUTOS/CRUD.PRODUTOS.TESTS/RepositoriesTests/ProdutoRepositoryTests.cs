using CRUD.PRODUTOS.DATA.Data;
using CRUD.PRODUTOS.DATA.Repositories;
using CRUD.PRODUTOS.DOMAIN.Repositories;
using CRUD.PRODUTOS.TESTS.Factories;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace CRUD.PRODUTOS.TESTS.RepositoriesTests;

public class ProdutoRepositoryTests : IDisposable
{
    private readonly AppDBContext _dbContext;
    private readonly IProdutoRepository _produtoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProdutoRepositoryTests()
    {
        _dbContext = TestAppDbContextFactory.Create();
        _produtoRepository = new ProdutoRepository(_dbContext);
        _unitOfWork = new UnitOfWork(_dbContext, TimeProvider.System);
    }

    public void Dispose() => _dbContext.Dispose();

    private async Task<int> SemearAsync(string nome = "Camiseta", decimal preco = 80m)
    {
        var produto = ProdutoBuilder.Entidade(nome: nome, preco: preco);

        await _produtoRepository.AdicionarAsync(produto);
        await _unitOfWork.CommitAsync();

        return produto.Id;
    }

    [Fact]
    public async Task Should_Criar_Produto()
    {
        //Act
        var id = await SemearAsync();

        //Assert
        id.ShouldBeGreaterThan(0);
        (await _dbContext.Produtos.CountAsync()).ShouldBe(1);
    }

    [Fact]
    public async Task Should_Preencher_Datas_De_Auditoria_Ao_Criar()
    {
        //Arrange: DataCriacao não é mais responsabilidade de quem chama —
        //sem isso a coluna "timestamp with time zone" recebia DateTime.MinValue.
        var antes = DateTime.UtcNow.AddSeconds(-1);

        //Act
        var id = await SemearAsync();

        //Assert
        var produto = await _dbContext.Produtos.AsNoTracking().SingleAsync(p => p.Id == id);
        produto.DataCriacao.ShouldBeGreaterThan(antes);
        produto.DataCriacao.Kind.ShouldBe(DateTimeKind.Utc);
        produto.DataAlteracao.ShouldBe(produto.DataCriacao);
    }

    [Fact]
    public async Task Should_Atualizar_DataAlteracao_Sem_Tocar_Na_DataCriacao()
    {
        //Arrange
        var id = await SemearAsync();
        var criacaoOriginal = (await _dbContext.Produtos.AsNoTracking().SingleAsync(p => p.Id == id)).DataCriacao;

        //Act
        var produto = await _produtoRepository.ObterPorIdAsync(id, rastrear: true);
        produto.ShouldNotBeNull();
        produto.Nome = "Camiseta Editada";
        await _unitOfWork.CommitAsync();

        //Assert
        var atualizado = await _dbContext.Produtos.AsNoTracking().SingleAsync(p => p.Id == id);
        atualizado.DataCriacao.ShouldBe(criacaoOriginal);
        atualizado.DataAlteracao.ShouldBeGreaterThanOrEqualTo(criacaoOriginal);
    }

    [Fact]
    public async Task Should_Listar_Produto_Por_Id()
    {
        //Arrange
        var id = await SemearAsync();

        //Act
        var produto = await _produtoRepository.ObterPorIdAsync(id);

        //Assert
        produto.ShouldNotBeNull();
        produto.Nome.ShouldBe("Camiseta");
    }

    [Fact]
    public async Task Should_Retornar_Null_Produto_Nao_Encontrado()
    {
        (await _produtoRepository.ObterPorIdAsync(99)).ShouldBeNull();
    }

    [Fact]
    public async Task Should_Retornar_Entidade_Sem_Rastreio_Por_Padrao()
    {
        //Arrange
        var id = await SemearAsync();
        _dbContext.ChangeTracker.Clear();

        //Act
        await _produtoRepository.ObterPorIdAsync(id);

        //Assert: consulta de leitura não deve sujar o change tracker
        _dbContext.ChangeTracker.Entries().ShouldBeEmpty();
    }

    [Fact]
    public async Task Should_Listar_Produtos_Paginado()
    {
        //Arrange
        for (var i = 1; i <= 5; i++)
            await SemearAsync(nome: $"Produto {i}");

        //Act
        var (itens, totalItens) = await _produtoRepository.BuscarAsync(null, page: 2, limit: 2);

        //Assert
        totalItens.ShouldBe(5);
        itens.Count.ShouldBe(2);
        itens[0].Nome.ShouldBe("Produto 3");
    }

    [Fact]
    public async Task Should_Remover_Produto()
    {
        //Arrange
        var id = await SemearAsync();
        var produto = await _produtoRepository.ObterPorIdAsync(id, rastrear: true);
        produto.ShouldNotBeNull();

        //Act
        _produtoRepository.Remover(produto);
        await _unitOfWork.CommitAsync();

        //Assert
        (await _dbContext.Produtos.CountAsync()).ShouldBe(0);
    }
}
