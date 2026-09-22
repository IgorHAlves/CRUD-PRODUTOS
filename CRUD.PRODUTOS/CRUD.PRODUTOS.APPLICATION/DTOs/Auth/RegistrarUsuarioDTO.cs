using System.ComponentModel.DataAnnotations;

namespace CRUD.PRODUTOS.APPLICATION.DTOs.Auth;

/// <summary>
/// Dados de auto-cadastro. O perfil (Role) NÃO é aceito do cliente:
/// todo usuário registrado por este endpoint nasce como "Padrao".
/// </summary>
public class RegistrarUsuarioDTO
{
    [Required(ErrorMessage = "O login é obrigatório.")]
    [StringLength(60, MinimumLength = 3, ErrorMessage = "O login deve ter entre 3 e 60 caracteres.")]
    public string Login { get; init; } = null!;

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "A senha deve ter entre 8 e 100 caracteres.")]
    public string Senha { get; init; } = null!;
}
