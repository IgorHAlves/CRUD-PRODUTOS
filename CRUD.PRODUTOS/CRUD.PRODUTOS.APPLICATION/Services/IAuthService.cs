using CRUD.PRODUTOS.APPLICATION.DTOs.Auth;

namespace CRUD.PRODUTOS.APPLICATION.Services;

public interface IAuthService
{
    /// <summary>
    /// Cadastra um novo usuário sempre com o perfil "Padrao".
    /// </summary>
    Task RegistrarAsync(RegistrarUsuarioDTO dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Autentica o usuário e emite o token JWT.
    /// </summary>
    /// <exception cref="DOMAIN.Exceptions.CredenciaisInvalidasException">Login ou senha inválidos.</exception>
    Task<TokenResponseDTO> LoginAsync(LoginDTO dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Altera o perfil de um usuário. Operação restrita a administradores.
    /// </summary>
    Task AlterarRoleAsync(int usuarioId, AlterarRoleDTO dto, CancellationToken cancellationToken = default);
}
