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
    
    /// <summary>
    /// Retorna um produto pelo Id.
    /// </summary>
    /// <param name="id">Identificador do produto.</param>
    /// <response code="200">Produto encontrado.</response>
    /// <response code="404">Produto não encontrado.</response>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var produto = await _produtoService.ListarProdutoAsync(id);
            return Ok(produto);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }
    
    /// <summary>
    /// Lista produtos de forma paginada.
    /// </summary>
    /// <param name="page">Página atual.</param>
    /// <param name="limit">Quantidade de itens por página.</param>
    /// <response code="200">Lista de produtos retornada.</response>
    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 10)
    {
        var produtos = await _produtoService.ListarProdutosAsync(page, limit);
        return Ok(produtos);
    }

    
    /// <summary>
    /// Cadastra um novo produto.
    /// </summary>
    /// <remarks>
    /// Valida se o preço e a quantidade são maiores ou iguais a zero.
    /// </remarks>
    /// <response code="201">Produto criado com sucesso.</response>
    /// <response code="400">Dados inválidos.</response>
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CriarProdutoDTO dto)
    {
        try
        {
            var id = await _produtoService.CriarProdutoAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id }, null);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    /// <summary>
    /// Atualiza os dados de um produto existente.
    /// </summary>
    /// <param name="id">Identificador do produto.</param>
    /// <response code="204">Produto atualizado com sucesso.</response>
    /// <response code="400">Dados inválidos.</response>
    /// <response code="404">Produto não encontrado.</response>
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] EditarProdutoDTO dto)
    {
        try
        {
            await _produtoService.EditarProdutoAsync(id, dto);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Remove um produto.
    /// </summary>
    /// <param name="id">Identificador do produto.</param>
    /// <response code="204">Produto removido com sucesso.</response>
    /// <response code="404">Produto não encontrado.</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _produtoService.DeletarProdutoAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

}