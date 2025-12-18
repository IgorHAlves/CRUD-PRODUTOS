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

    public ProdutoRepository(AppDBContext dbContext)
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

    public async Task<VisualizarLista<Produto>> ListarProdutosAsync(string nomeProduto, int page, int limit)
    {
        var query = _dbContext.Produtos.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(nomeProduto))
        {
            query = query.Where(p => p.Nome.ToLower().Contains(nomeProduto.ToLower()));
        }

        int totalItens = await query.CountAsync();

        var produtos = await query
            .OrderBy(p => p.Id) 
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();

        return new VisualizarLista<Produto>
        {
            TotalItens = totalItens,
            TotalPaginas = (int)Math.Ceiling(totalItens / (double)limit),
            PaginaAtual = page,
            Itens = produtos
        };
    }

    public async Task EditarProdutoAsync(int id, EditarProdutoDTO dto)
    {
        var produto = await _dbContext.Produtos.FirstOrDefaultAsync(p => p.Id == id);

        if (produto == null)
            throw new KeyNotFoundException("Produto não encontrado");

        produto.Nome = dto.Nome;
        produto.Descricao = dto.Descricao;
        produto.Preco = dto.Preco;
        produto.QuantidadeEmEstoque = dto.QuantidadeEmEstoque;
        produto.DataAlteracao = DateTime.UtcNow;
    }


    public async Task DeletarProdutoAsync(int id)
    {
        var produto = await _dbContext.Produtos.FindAsync(id);
        if (produto == null)
            throw new KeyNotFoundException("Produto não encontrado");
        _dbContext.Produtos.Remove(produto);
    }

}