using CRUD.PRODUTOS.DOMAIN.DTOs;
using CRUD.PRODUTOS.DOMAIN.Helper;
using CRUD.PRODUTOS.DOMAIN.Models;

namespace CRUD.PRODUTOS.INTERFACES;

public interface IProdutoRepository
{
    Task CriarProdutoAsync(Produto produto);
    Task<Produto> ListarProdutoAsync(int id);
    Task<VisualizarLista<Produto>> ListarProdutosAsync(int page, int limit);
    Task EditarProdutoAsync(int id, EditarProdutoDTO editarProdutoDTO);
    Task DeletarProdutoAsync(int id);
}