using CRUD.PRODUTOS.DOMAIN.DTOs;
using CRUD.PRODUTOS.DOMAIN.Helper;
using CRUD.PRODUTOS.DOMAIN.Models;
using CRUD.PRODUTOS.INTERFACES;

namespace CRUD.PRODUTOS.SERVICES;

public class ProdutoService : IProdutoService
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProdutoService(
        IProdutoRepository produtoRepository,
        IUnitOfWork unitOfWork)
    {
        _produtoRepository = produtoRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<VisualizarProdutoDTO?> ListarProdutoAsync(int id)
    {
        Produto? produto = await _produtoRepository.ListarProdutoAsync(id);

        if (produto == null)
            throw new ArgumentException("Produto não encontrado");
        
        VisualizarProdutoDTO retorno = new VisualizarProdutoDTO()
        {
            Id = produto.Id,
            Nome = produto.Nome,
            Descricao = produto.Descricao,
            Preco = produto.Preco,
            QuantidadeEmEstoque = produto.QuantidadeEmEstoque
        };
        
        return retorno;
    }

    public async Task<VisualizarLista<VisualizarProdutoDTO>> ListarProdutosAsync(int page, int limit)
    {
        var resultado = await _produtoRepository.ListarProdutosAsync(page, limit);

        return new VisualizarLista<VisualizarProdutoDTO>
        {
            TotalItens = resultado.TotalItens,
            TotalPaginas = resultado.TotalPaginas,
            PaginaAtual = resultado.PaginaAtual,
            Itens = resultado.Itens.Select(p => new VisualizarProdutoDTO
            {
                Id = p.Id,
                Nome = p.Nome,
                Descricao = p.Descricao,
                Preco = p.Preco,
                QuantidadeEmEstoque = p.QuantidadeEmEstoque
            }).ToList()
        };
    }

    public async Task<int> CriarProdutoAsync(CriarProdutoDTO dto)
    {
        if (dto.Preco < 0)
            throw new ArgumentException("Preço não pode ser negativo");
        
        if (dto.Preco == 0)
            throw new ArgumentException("Preço não pode ser zero");
        
        if (dto.QuantidadeEmEstoque < 0)
            throw new ArgumentException("Quantidade não pode ser negativa");
        
        var produto = new Produto
        {
            Nome = dto.Nome,
            Descricao = dto.Descricao,
            Preco = dto.Preco,
            QuantidadeEmEstoque = dto.QuantidadeEmEstoque,
        };

        await _produtoRepository.CriarProdutoAsync(produto);
        await _unitOfWork.CommitAsync();

        return produto.Id;
    }

    public async Task EditarProdutoAsync(int id,EditarProdutoDTO dto)
    {
        if (dto.Preco < 0)
            throw new ArgumentException("Preço não pode ser negativo");

        if (dto.QuantidadeEmEstoque < 0)
            throw new ArgumentException("Quantidade não pode ser negativa");
        
        await _produtoRepository.EditarProdutoAsync(id,dto);
        await _unitOfWork.CommitAsync();
    }

    public async Task DeletarProdutoAsync(int id)
    {
        await _produtoRepository.DeletarProdutoAsync(id);
        await _unitOfWork.CommitAsync();
    }
}