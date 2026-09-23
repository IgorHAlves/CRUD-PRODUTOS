using CRUD.PRODUTOS.APPLICATION.Common;
using CRUD.PRODUTOS.APPLICATION.DTOs.Produto;
using CRUD.PRODUTOS.APPLICATION.Mappings;
using CRUD.PRODUTOS.DOMAIN.Exceptions;
using CRUD.PRODUTOS.DOMAIN.Repositories;

namespace CRUD.PRODUTOS.APPLICATION.Services;

public class ProdutoService : IProdutoService
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProdutoService(IProdutoRepository produtoRepository, IUnitOfWork unitOfWork)
    {
        _produtoRepository = produtoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<VisualizarProdutoDTO> ListarProdutoAsync(int id, CancellationToken cancellationToken = default)
    {
        var produto = await _produtoRepository.ObterPorIdAsync(id, cancellationToken: cancellationToken)
                      ?? throw NaoEncontradoException.Produto(id);
        
        //Como produtomapping é static e passa um produto como parâmetro, posso chamar produto.metodo-do-produtomapping
        return produto.ParaVisualizacao();
    }

    public async Task<ResultadoPaginado<VisualizarProdutoDTO>> ListarProdutosAsync(
        FiltroProdutoDTO filtro,
        CancellationToken cancellationToken = default)
    {
        var (itens, totalItens) = await _produtoRepository.BuscarAsync(
            filtro.NomeProduto,
            filtro.Page,
            filtro.Limit,
            cancellationToken);

        return ResultadoPaginado<VisualizarProdutoDTO>.Criar(
            itens.ParaVisualizacao(),
            totalItens,
            filtro.Page,
            filtro.Limit);
    }

    public async Task<int> CriarProdutoAsync(CriarProdutoDTO dto, CancellationToken cancellationToken = default)
    {
        var produto = dto.ParaEntidade();

        await _produtoRepository.AdicionarAsync(produto, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return produto.Id;
    }

    public async Task EditarProdutoAsync(int id, EditarProdutoDTO dto, CancellationToken cancellationToken = default)
    {
        var produto = await _produtoRepository.ObterPorIdAsync(id, rastrear: true, cancellationToken)
                      ?? throw NaoEncontradoException.Produto(id);

        dto.AplicarEm(produto);

        await _unitOfWork.CommitAsync(cancellationToken);
    }

    public async Task DeletarProdutoAsync(int id, CancellationToken cancellationToken = default)
    {
        var produto = await _produtoRepository.ObterPorIdAsync(id, rastrear: true, cancellationToken)
                      ?? throw NaoEncontradoException.Produto(id);

        _produtoRepository.Remover(produto);

        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
