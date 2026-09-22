using CRUD.PRODUTOS.APPLICATION.Common;
using CRUD.PRODUTOS.APPLICATION.DTOs.Produto;
using CRUD.PRODUTOS.APPLICATION.Services;
using CRUD.PRODUTOS.DOMAIN.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRUD.PRODUTOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
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
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Produto encontrado.</response>
    /// <response code="404">Produto não encontrado.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(VisualizarProdutoDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VisualizarProdutoDTO>> GetById(int id, CancellationToken cancellationToken)
    {
        return Ok(await _produtoService.ListarProdutoAsync(id, cancellationToken));
    }

    /// <summary>
    /// Lista produtos de forma paginada, opcionalmente filtrando pelo nome.
    /// </summary>
    /// <param name="filtro">Nome, página e quantidade de itens por página.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Lista de produtos retornada.</response>
    /// <response code="400">Parâmetros de paginação inválidos.</response>
    [HttpGet]
    [ProducesResponseType(typeof(ResultadoPaginado<VisualizarProdutoDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ResultadoPaginado<VisualizarProdutoDTO>>> Get(
        [FromQuery] FiltroProdutoDTO filtro,
        CancellationToken cancellationToken)
    {
        return Ok(await _produtoService.ListarProdutosAsync(filtro, cancellationToken));
    }

    /// <summary>
    /// Cadastra um novo produto.
    /// </summary>
    /// <param name="dto">Dados do produto.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="201">Produto criado com sucesso.</response>
    /// <response code="400">Dados inválidos.</response>
    /// <response code="403">Usuário autenticado não é administrador.</response>
    [Authorize(Roles = Roles.Admin)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CriarProdutoDTO dto, CancellationToken cancellationToken)
    {
        var id = await _produtoService.CriarProdutoAsync(dto, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id }, null);
    }

    /// <summary>
    /// Atualiza os dados de um produto existente.
    /// </summary>
    /// <param name="id">Identificador do produto.</param>
    /// <param name="dto">Novos dados do produto.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="204">Produto atualizado com sucesso.</response>
    /// <response code="400">Dados inválidos.</response>
    /// <response code="403">Usuário autenticado não é administrador.</response>
    /// <response code="404">Produto não encontrado.</response>
    [Authorize(Roles = Roles.Admin)]
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Put(int id, [FromBody] EditarProdutoDTO dto, CancellationToken cancellationToken)
    {
        await _produtoService.EditarProdutoAsync(id, dto, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Remove um produto.
    /// </summary>
    /// <param name="id">Identificador do produto.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="204">Produto removido com sucesso.</response>
    /// <response code="403">Usuário autenticado não é administrador.</response>
    /// <response code="404">Produto não encontrado.</response>
    [Authorize(Roles = Roles.Admin)]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _produtoService.DeletarProdutoAsync(id, cancellationToken);

        return NoContent();
    }
}
