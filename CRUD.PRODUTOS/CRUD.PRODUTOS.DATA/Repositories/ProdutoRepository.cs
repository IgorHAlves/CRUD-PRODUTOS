using CRUD.PRODUTOS.DATA.Data;
using CRUD.PRODUTOS.DOMAIN.DTOs;
using CRUD.PRODUTOS.DOMAIN.Helper;
using CRUD.PRODUTOS.DOMAIN.Models;
using CRUD.PRODUTOS.INTERFACES;
using Microsoft.EntityFrameworkCore;

namespace CRUD.PRODUTOS.DATA.Repositories;

public class ProdutoRepository : IProdutoRepository
{
    private readonly AppDBContext _dbContext;

    public ProdutoRepository(AppDBContext dbContext, IUnitOfWork unitOfWork)
    {
        _dbContext = dbContext;
    }
    
    public async Task CriarProdutoAsync(Produto produto)
    {
        await _dbContext.Produtos.AddAsync(produto);
    }


    public async Task<Produto?> ListarProdutoAsync(int id)
    {
        return await _dbContext.Produtos
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<VisualizarLista<VisualizarProdutoDTO>> ListarProdutosAsync(int page, int limit)
    {
        var query = _dbContext.Produtos.AsNoTracking();

        int totalItens = await query.CountAsync();

        List<VisualizarProdutoDTO> produtos = await query
            .Skip((page - 1) * limit)
            .Take(limit)
            .Select(p => new VisualizarProdutoDTO
            {
                Id = p.Id,
                Nome = p.Nome,
                Descricao = p.Descricao,
                Preco = p.Preco,
                QuantidadeEmEstoque = p.QuantidadeEmEstoque
            })
            .ToListAsync();

        return new VisualizarLista<VisualizarProdutoDTO>
        {
            TotalItens = totalItens,
            TotalPaginas = (int)Math.Ceiling(totalItens / (double)limit),
            PaginaAtual = page,
            Itens = produtos
        };
    }

    public Task EditarProdutoAsync(Produto produto)
    {
        _dbContext.Produtos.Update(produto);
        return Task.CompletedTask;
    }

    public async Task DeletarProdutoAsync(int id)
    {
        var produto = await _dbContext.Produtos.FindAsync(id);
        if (produto != null)
            _dbContext.Produtos.Remove(produto);
    }

}