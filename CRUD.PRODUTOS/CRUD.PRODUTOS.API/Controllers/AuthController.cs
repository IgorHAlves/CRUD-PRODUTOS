using CRUD.PRODUTOS.APPLICATION.DTOs.Auth;
using CRUD.PRODUTOS.APPLICATION.Services;
using CRUD.PRODUTOS.DOMAIN.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRUD.PRODUTOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Cadastra um novo usuário com o perfil padrão.
    /// </summary>
    /// <remarks>
    /// O perfil de acesso não é aceito neste endpoint: todo usuário criado aqui
    /// nasce como "Padrao". A promoção a administrador é feita em
    /// <c>PUT /api/auth/usuarios/{id}/role</c> por um administrador.
    /// </remarks>
    /// <response code="204">Usuário cadastrado.</response>
    /// <response code="400">Dados inválidos ou login já cadastrado.</response>
    [HttpPost("registrar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Registrar(RegistrarUsuarioDTO dto, CancellationToken cancellationToken)
    {
        await _authService.RegistrarAsync(dto, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Autentica o usuário e devolve o token JWT.
    /// </summary>
    /// <response code="200">Token emitido.</response>
    /// <response code="401">Login ou senha inválidos.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(TokenResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TokenResponseDTO>> Login(LoginDTO dto, CancellationToken cancellationToken)
    {
        return Ok(await _authService.LoginAsync(dto, cancellationToken));
    }

    /// <summary>
    /// Altera o perfil de acesso de um usuário. Restrito a administradores.
    /// </summary>
    /// <param name="id">Identificador do usuário.</param>
    /// <param name="dto">Novo perfil ("Admin" ou "Padrao").</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="204">Perfil alterado.</response>
    /// <response code="400">Perfil inválido.</response>
    /// <response code="401">Token ausente ou inválido.</response>
    /// <response code="403">Usuário autenticado não é administrador.</response>
    /// <response code="404">Usuário não encontrado.</response>
    [Authorize(Roles = Roles.Admin)]
    [HttpPut("usuarios/{id:int}/role")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AlterarRole(int id, AlterarRoleDTO dto, CancellationToken cancellationToken)
    {
        await _authService.AlterarRoleAsync(id, dto, cancellationToken);

        return NoContent();
    }
}
