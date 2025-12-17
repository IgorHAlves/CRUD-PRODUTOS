using CRUD.PRODUTOS.DATA.Data;
using CRUD.PRODUTOS.DATA.Repositories;
using CRUD.PRODUTOS.DOMAIN.DTOs;
using CRUD.PRODUTOS.DOMAIN.Helper;
using CRUD.PRODUTOS.INTERFACES;
using CRUD.PRODUTOS.SERVICES;
using Shouldly;
using Xunit;

namespace CRUD.PRODUTOS.TESTS.ServicesTests;

public class ProdutoServiceTests
{
    private readonly AppDBContext _dbContext;
    private readonly IProdutoService _produtoService;

    public ProdutoServiceTests()
    {
        _dbContext = TestAppDbContextFactory.Create();

        var produtoRepository = new ProdutoRepository(_dbContext);
        var unitOfWork = new UnitOfWork(_dbContext);

        _produtoService = new ProdutoService(produtoRepository, unitOfWork);
    }

    [Fact]
    public async Task Should_Criar_Produto()
    {
        //Arrange
        CriarProdutoDTO criarProdutoDTO = new CriarProdutoDTO()
        {
            Nome = "Camiseta",
            Descricao = "Camiseta de algodão",
            Preco = 80,
            QuantidadeEmEstoque = 20
        };
        
        //Act
        int idNovoProduto = await _produtoService.CriarProdutoAsync(criarProdutoDTO);
        
        //Asset
        idNovoProduto.ShouldBe(1);
    }

    [Fact]
    public async Task Should_Throw_Criar_Produto_Preco_Negativo()
    {
        
        //Arrange
        CriarProdutoDTO criarProdutoDTO = new CriarProdutoDTO()
        {
            Nome = "Camiseta",
            Descricao = "Camiseta de algodão",
            Preco = -80,
            QuantidadeEmEstoque = 20
        };
        
        //Act
        var ex = await Should.ThrowAsync<ArgumentException>(async () =>
        {
             await _produtoService.CriarProdutoAsync(criarProdutoDTO);
        });
        
        //Assert
        ex.Message.ShouldBe($"Preço não pode ser negativo");
    }
    
    [Fact]
    public async Task Should_Throw_Criar_Produto_Preco_Zero()
    {
        
        //Arrange
        CriarProdutoDTO criarProdutoDTO = new CriarProdutoDTO()
        {
            Nome = "Camiseta",
            Descricao = "Camiseta de algodão",
            Preco = 0,
            QuantidadeEmEstoque = 20
        };
        
        //Act
        var ex = await Should.ThrowAsync<ArgumentException>(async () =>
        {
            await _produtoService.CriarProdutoAsync(criarProdutoDTO);
        });
        
        //Assert
        ex.Message.ShouldBe($"Preço não pode ser zero");
    }
    
    [Fact]
    public async Task Should_Throw_Criar_Produto_Quantidade_Negativa()
    {
        
        //Arrange
        CriarProdutoDTO criarProdutoDTO = new CriarProdutoDTO()
        {
            Nome = "Camiseta",
            Descricao = "Camiseta de algodão",
            Preco = 80,
            QuantidadeEmEstoque = -20
        };
        
        //Act
        var ex = await Should.ThrowAsync<ArgumentException>(async () =>
        {
            await _produtoService.CriarProdutoAsync(criarProdutoDTO);
        });
        
        //Assert
        ex.Message.ShouldBe($"Quantidade não pode ser negativa");
    }
    
    [Fact]
    public async Task Should_Visualizar_Produto()
    {
        //Arrange
        CriarProdutoDTO criarProdutoDTO = new CriarProdutoDTO()
        {
            Nome = "Camiseta",
            Descricao = "Camiseta de algodão",
            Preco = 80,
            QuantidadeEmEstoque = 20
        };
        
        //Act
        int idNovoProduto = await _produtoService.CriarProdutoAsync(criarProdutoDTO);
        
        VisualizarProdutoDTO visualizarProdutoDTO = await _produtoService.ListarProdutoAsync(idNovoProduto);   
        
        //Asset
        visualizarProdutoDTO.Id.ShouldBe(1);
        visualizarProdutoDTO.Nome.ShouldBe(criarProdutoDTO.Nome);
        visualizarProdutoDTO.Descricao.ShouldBe(criarProdutoDTO.Descricao);
        visualizarProdutoDTO.Preco.ShouldBe(criarProdutoDTO.Preco);
        visualizarProdutoDTO.QuantidadeEmEstoque.ShouldBe(criarProdutoDTO.QuantidadeEmEstoque);
    }
    
    [Fact]
    public async Task Should_Throw_Visualizar_Produto_Id_Inexistente()
    {
        //Act
        var ex = await Should.ThrowAsync<ArgumentException>(async () =>
        {
            await _produtoService.ListarProdutoAsync(1);
        });
        
        //Assert
        ex.Message.ShouldBe($"Produto não encontrado");
    }

    
    [Fact]
    public async Task Should_Visualizar_Lista_Produtos()
    {
        //Arrange
        CriarProdutoDTO criarProdutoDTO1 = new CriarProdutoDTO()
        {
            Nome = "Camiseta",
            Descricao = "Camiseta de algodão",
            Preco = 80,
            QuantidadeEmEstoque = 20
        };
        
        CriarProdutoDTO criarProdutoDTO2 = new CriarProdutoDTO()
        {
            Nome = "Camiseta2",
            Descricao = "Camiseta de algodão2",
            Preco = 60,
            QuantidadeEmEstoque = 10
        };
        
        //Act
        int idNovoProduto1 = await _produtoService.CriarProdutoAsync(criarProdutoDTO1);
        int idNovoProduto2 = await _produtoService.CriarProdutoAsync(criarProdutoDTO2);
        
        VisualizarLista<VisualizarProdutoDTO> visualizarProdutoDTO = await _produtoService.ListarProdutosAsync();   
        
        //Asset
        visualizarProdutoDTO.Itens[0].Id.ShouldBe(1);
        visualizarProdutoDTO.Itens[0].Nome.ShouldBe(criarProdutoDTO1.Nome);
        visualizarProdutoDTO.Itens[0].Descricao.ShouldBe(criarProdutoDTO1.Descricao);
        visualizarProdutoDTO.Itens[0].Preco.ShouldBe(criarProdutoDTO1.Preco);
        visualizarProdutoDTO.Itens[0].QuantidadeEmEstoque.ShouldBe(criarProdutoDTO1.QuantidadeEmEstoque);
        
        visualizarProdutoDTO.Itens[1].Id.ShouldBe(2);
        visualizarProdutoDTO.Itens[1].Nome.ShouldBe(criarProdutoDTO2.Nome);
        visualizarProdutoDTO.Itens[1].Descricao.ShouldBe(criarProdutoDTO2.Descricao);
        visualizarProdutoDTO.Itens[1].Preco.ShouldBe(criarProdutoDTO2.Preco);
        visualizarProdutoDTO.Itens[1].QuantidadeEmEstoque.ShouldBe(criarProdutoDTO2.QuantidadeEmEstoque);
        
        visualizarProdutoDTO.PaginaAtual.ShouldBe(1);
        visualizarProdutoDTO.TotalPaginas.ShouldBe(1);
        visualizarProdutoDTO.TotalItens.ShouldBe(2);
    }
    
    [Fact]
    public async Task Should_Editar_Produto()
    {
        //Arrange
        CriarProdutoDTO criarProdutoDTO = new CriarProdutoDTO()
        {
            Nome = "Camiseta",
            Descricao = "Camiseta de algodão",
            Preco = 80,
            QuantidadeEmEstoque = 20
        };
        
        int idNovoProduto = await _produtoService.CriarProdutoAsync(criarProdutoDTO);

        EditarProdutoDTO editarProdutoDTO = new EditarProdutoDTO()
        {
            Nome = "Camiseta Editada",
            Descricao = "",
            Preco = 50,
            QuantidadeEmEstoque = 10
        };
        
        //Act
        _produtoService.EditarProdutoAsync(idNovoProduto,editarProdutoDTO);
        
        //Asset
        VisualizarProdutoDTO visualizarProdutoDto = await _produtoService.ListarProdutoAsync(idNovoProduto);
        
        visualizarProdutoDto.Nome.ShouldBe(editarProdutoDTO.Nome);
        visualizarProdutoDto.Descricao.ShouldBe(editarProdutoDTO.Descricao);
        visualizarProdutoDto.Preco.ShouldBe(editarProdutoDTO.Preco);
        visualizarProdutoDto.QuantidadeEmEstoque.ShouldBe(editarProdutoDTO.QuantidadeEmEstoque);
    }
    
    [Fact]
    public async Task Should_Editar_Produto_Nao_Encontrado()
    {
        //Arrange
        EditarProdutoDTO editarProdutoDTO = new EditarProdutoDTO()
        {
            Nome = "Camiseta Editada",
            Descricao = "",
            Preco = 50,
            QuantidadeEmEstoque = 10
        };
        
        //Act
        var ex = await Should.ThrowAsync<KeyNotFoundException>(async () =>
        {
            await _produtoService.EditarProdutoAsync(1,editarProdutoDTO);
        });
        
        //Assert
        ex.Message.ShouldBe($"Produto não encontrado");
    }
    
    [Fact]
    public async Task Should_Deletar_Produto()
    {
        //Arrange
        CriarProdutoDTO criarProdutoDTO1 = new CriarProdutoDTO()
        {
            Nome = "Camiseta",
            Descricao = "Camiseta de algodão",
            Preco = 80,
            QuantidadeEmEstoque = 20
        };
        
        CriarProdutoDTO criarProdutoDTO2 = new CriarProdutoDTO()
        {
            Nome = "Camiseta2",
            Descricao = "Camiseta de algodão2",
            Preco = 60,
            QuantidadeEmEstoque = 10
        };
        
        int idNovoProduto1 = await _produtoService.CriarProdutoAsync(criarProdutoDTO1);
        int idNovoProduto2 = await _produtoService.CriarProdutoAsync(criarProdutoDTO2);
        
        //Act
        _produtoService.DeletarProdutoAsync(idNovoProduto1);
        //Asset
        VisualizarLista<VisualizarProdutoDTO> produtos =  await _produtoService.ListarProdutosAsync();
        
        produtos.Itens.Count.ShouldBe(1);
        produtos.TotalItens.ShouldBe(1);
    }
    
    [Fact]
    public async Task Should_Deletar_Produto_Nao_Encontrado()
    {
        //Act
        var ex = await Should.ThrowAsync<KeyNotFoundException>(async () =>
        {
            await _produtoService.DeletarProdutoAsync(1);
        });
        
        //Assert
        ex.Message.ShouldBe($"Produto não encontrado");
    }
}