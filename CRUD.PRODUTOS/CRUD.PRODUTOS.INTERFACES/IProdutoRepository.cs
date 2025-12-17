using CRUD.PRODUTOS.DOMAIN.DTOs;
using CRUD.PRODUTOS.DOMAIN.Helper;
using CRUD.PRODUTOS.DOMAIN.Models;

namespace CRUD.PRODUTOS.INTERFACES;

public interface IProdutoRepository
{
    Task CriarProdutoAsync(Produto produto);
    Task<Produto> ListarProdutoAsync(int Id);
    Task<VisualizarLista<VisualizarProdutoDTO>> ListarProdutosAsync(int page, int limit);
    Task EditarProdutoAsync(Produto produto);
    Task DeletarProdutoAsync(int Id);
}