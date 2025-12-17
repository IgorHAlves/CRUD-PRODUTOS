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

    public async Task<int> CriarProdutoAsync(CriarProdutoDTO dto)
    {
        var produto = new Produto
        {
            Nome = dto.Nome,
            Descricao = dto.Descricao,
            Preco = dto.Preco,
            QuantidadeEmEstoque = dto.QuantidadeEmEstoque,
            DataCriacao = DateTime.UtcNow
        };

        await _produtoRepository.CriarProdutoAsync(produto);
        await _unitOfWork.CommitAsync();

        return produto.Id;
    }

    public async Task EditarProdutoAsync(EditarProdutoDTO dto)
    {
        var produto = new Produto
        {
            Id = dto.Id,
            Nome = dto.Nome,
            Descricao = dto.Descricao,
            Preco = dto.Preco,
            QuantidadeEmEstoque = dto.QuantidadeEmEstoque,
            DataAlteracao = DateTime.UtcNow
        };

        await _produtoRepository.EditarProdutoAsync(produto);
        await _unitOfWork.CommitAsync();
    }

    public async Task DeletarProdutoAsync(int id)
    {
        await _produtoRepository.DeletarProdutoAsync(id);
        await _unitOfWork.CommitAsync();
    }

    public async Task<VisualizarProdutoDTO?> ListarProdutoAsync(int id)
    {
        Produto? produto = await _produtoRepository.ListarProdutoAsync(id);

        VisualizarProdutoDTO retorno = new VisualizarProdutoDTO()
        {
            Id = produto.Id,
            Nome = produto.Nome,
            Descricao = produto.Descricao,
            Preco = produto.Preco,
        };
        
        return retorno;
    }

    public async Task<VisualizarLista<VisualizarProdutoDTO>> ListarProdutosAsync(int page, int limit)
    {
        return await _produtoRepository.ListarProdutosAsync(page, limit);
    }
}