using CRUD.PRODUTOS.DOMAIN.DTOs;
using CRUD.PRODUTOS.DOMAIN.Helper;
using CRUD.PRODUTOS.INTERFACES;
using Microsoft.AspNetCore.Mvc;

namespace CRUD.PRODUTOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutoController : ControllerBase
{
    private readonly IProdutoService _produtoService;

    public ProdutoController(IProdutoService produtoService)
    {
        _produtoService = produtoService;
    }

    [HttpPost]
    public async Task<IActionResult> CriarProduto([FromBody] CriarProdutoDTO dto)
    {
        int idProduto = await _produtoService.CriarProdutoAsync(dto);
        
        return CreatedAtAction(nameof(ListarProduto), new { id = idProduto }, idProduto);
    }

    [HttpPut]
    public async Task<IActionResult> EditarProduto([FromBody] EditarProdutoDTO dto)
    {
        await _produtoService.EditarProdutoAsync(dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletarProduto(int id)
    {
        await _produtoService.DeletarProdutoAsync(id);
        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ListarProduto(int id)
    {
        var produto = await _produtoService.ListarProdutoAsync(id);

        if (produto == null)
            return NotFound();

        return Ok(produto);
    }

    [HttpGet]
    public async Task<IActionResult> ListarProdutos(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 10)
    {
        var produtos = await _produtoService.ListarProdutosAsync(page, limit);
        return Ok(produtos);
    }
}