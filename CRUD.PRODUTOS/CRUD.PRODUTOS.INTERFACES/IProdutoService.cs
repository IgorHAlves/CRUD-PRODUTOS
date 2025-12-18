using CRUD.PRODUTOS.DOMAIN.DTOs;
using CRUD.PRODUTOS.DOMAIN.Helper;
using CRUD.PRODUTOS.DOMAIN.Models;

namespace CRUD.PRODUTOS.INTERFACES;

public interface IProdutoService
{
    Task<int> CriarProdutoAsync(CriarProdutoDTO dto);

    Task EditarProdutoAsync(int id,EditarProdutoDTO dto);

    Task DeletarProdutoAsync(int id);

    Task<VisualizarProdutoDTO?> ListarProdutoAsync(int id);

    Task<VisualizarLista<VisualizarProdutoDTO>> ListarProdutosAsync(string? nomeProduto = "",int page = 1, int limit = 10);
}
