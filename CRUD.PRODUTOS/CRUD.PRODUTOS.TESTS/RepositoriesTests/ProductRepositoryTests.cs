using CRUD.PRODUTOS.DATA.Data;
using CRUD.PRODUTOS.DATA.Repositories;
using CRUD.PRODUTOS.DOMAIN.DTOs;
using CRUD.PRODUTOS.DOMAIN.Models;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace CRUD.PRODUTOS.TESTS.RepositoriesTests;

public class ProdutoRepositoryTests
{
    private readonly AppDBContext _context;
    private readonly ProdutoRepository _repository;

    public ProdutoRepositoryTests()
    {
        _context = TestAppDbContextFactory.Create();
        _repository = new ProdutoRepository(_context);
    }

    [Fact]
    public async Task Should_Criar_Produto()
    {
        var produto = new Produto
        {
            Nome = "Camiseta",
            Preco = 50,
            QuantidadeEmEstoque = 10
        };

        await _repository.CriarProdutoAsync(produto);
        await _context.SaveChangesAsync();

        var salvo = await _context.Produtos.FirstOrDefaultAsync();

        salvo.ShouldNotBeNull();
        salvo!.Nome.ShouldBe("Camiseta");
    }

    [Fact]
    public async Task Should_Listar_Produto_Por_Id()
    {
        _context.Produtos.Add(new Produto
        {
            Nome = "Produto",
            Preco = 100,
            QuantidadeEmEstoque = 5
        });
        await _context.SaveChangesAsync();

        var produto = await _repository.ListarProdutoAsync(1);

        produto.ShouldNotBeNull();
        produto!.Nome.ShouldBe("Produto");
    }

    [Fact]
    public async Task Should_Retornar_Null_Produto_Nao_Enontrado()
    {
        Produto? produto = await _repository.ListarProdutoAsync(999);

        produto.ShouldBeNull();
    }

    [Fact]
    public async Task Should_Listar_Produtos_Paginado()
    {
        for (int i = 1; i <= 10; i++)
        {
            _context.Produtos.Add(new Produto
            {
                Nome = $"Produto {i}",
                Preco = i * 10,
                QuantidadeEmEstoque = i
            });
        }
        await _context.SaveChangesAsync();

        var resultado = await _repository.ListarProdutosAsync("",page: 2, limit: 3);

        resultado.TotalItens.ShouldBe(10);
        resultado.TotalPaginas.ShouldBe(4);
        resultado.PaginaAtual.ShouldBe(2);
        resultado.Itens.Count.ShouldBe(3);
    }
    
    [Fact]
    public async Task Should_Listar_Produtos_Paginado_Filtro_Nome()
    {
        for (int i = 1; i <= 10; i++)
        {
            _context.Produtos.Add(new Produto
            {
                Nome = $"Produto {i}",
                Preco = i * 10,
                QuantidadeEmEstoque = i
            });
        }
        await _context.SaveChangesAsync();

        var resultado = await _repository.ListarProdutosAsync("Produto 2",page: 1, limit: 3);
        
        resultado.TotalItens.ShouldBe(1);
        resultado.TotalPaginas.ShouldBe(4);
        resultado.PaginaAtual.ShouldBe(1);
        resultado.Itens.Count.ShouldBe(1);
    }
    [Fact]
    public async Task Should_Editar_Produto()
    {
        _context.Produtos.Add(new Produto
        {
            Nome = "Produto",
            Preco = 20,
            QuantidadeEmEstoque = 5
        });
        await _context.SaveChangesAsync();

        var dto = new EditarProdutoDTO
        {
            Nome = "Produto Editado",
            Descricao = "Descricao Editada",
            Preco = 30,
            QuantidadeEmEstoque = 10
        };

        await _repository.EditarProdutoAsync(1, dto);
        await _context.SaveChangesAsync();

        var atualizado = await _context.Produtos.FindAsync(1);

        atualizado.ShouldNotBeNull();
        atualizado!.Nome.ShouldBe("Produto Editado");
        atualizado.Preco.ShouldBe(30);
        atualizado.QuantidadeEmEstoque.ShouldBe(10);
    }

    [Fact]
    public async Task Should_Throw_Editar_Produto_Nao_Encontrado()
    {
        var dto = new EditarProdutoDTO
        {
            Nome = "Teste",
            Preco = 10,
            QuantidadeEmEstoque = 1
        };

        var ex = await Should.ThrowAsync<KeyNotFoundException>(async () =>
        {
            await _repository.EditarProdutoAsync(1, dto);
        });

        ex.Message.ShouldBe("Produto não encontrado");
    }

    [Fact]
    public async Task Should_Deletar_Produto()
    {
        _context.Produtos.Add(new Produto
        {
            Nome = "Produto",
            Preco = 10,
            QuantidadeEmEstoque = 1
        });
        await _context.SaveChangesAsync();

        await _repository.DeletarProdutoAsync(1);
        await _context.SaveChangesAsync();

        var produto = await _context.Produtos.FindAsync(1);
        produto.ShouldBeNull();
    }

    [Fact]
    public async Task Should_Throw_When_Deletar_Produto_Not_Found()
    {
        var ex = await Should.ThrowAsync<KeyNotFoundException>(async () =>
        {
            await _repository.DeletarProdutoAsync(1);
        });

        ex.Message.ShouldBe("Produto não encontrado");
    }
}
