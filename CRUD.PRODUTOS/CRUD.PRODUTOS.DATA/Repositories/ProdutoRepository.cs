using CRUD.PRODUTOS.DATA.Data;
using CRUD.PRODUTOS.DOMAIN.Models;
using CRUD.PRODUTOS.DOMAIN.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CRUD.PRODUTOS.DATA.Repositories;

public class ProdutoRepository : IProdutoRepository
{
    private readonly AppDBContext _dbContext;

    public ProdutoRepository(AppDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AdicionarAsync(Produto produto, CancellationToken cancellationToken = default)
    {
        // Add (e não AddAsync): AddAsync só é necessário para geradores de valor
        // que consultam o banco, como o HiLo. Aqui a identity é resolvida no insert.
        _dbContext.Produtos.Add(produto);

        return Task.CompletedTask;
    }

    public async Task<Produto?> ObterPorIdAsync(
        int id,
        bool rastrear = false,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Produtos.AsQueryable();

        if (!rastrear)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<(IReadOnlyList<Produto> Itens, int TotalItens)> BuscarAsync(
        string? nomeProduto,
        int page,
        int limit,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Produtos.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(nomeProduto))
        {
            // ILIKE é resolvido pelo Postgres e, ao contrário de LOWER(...) LIKE,
            // permite o uso de índice (com pg_trgm) na coluna Nome.
            var termo = $"%{nomeProduto.Trim()}%";
            query = query.Where(p => EF.Functions.ILike(p.Nome, termo));
        }

        var totalItens = await query.CountAsync(cancellationToken);

        var itens = await query
            .OrderBy(p => p.Id)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync(cancellationToken);

        return (itens, totalItens);
    }

    public void Remover(Produto produto)
    {
        _dbContext.Produtos.Remove(produto);
    }
}
