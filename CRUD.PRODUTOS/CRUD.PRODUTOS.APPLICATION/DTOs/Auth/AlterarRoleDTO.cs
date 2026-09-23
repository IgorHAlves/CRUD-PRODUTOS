using System.ComponentModel.DataAnnotations;

namespace CRUD.PRODUTOS.APPLICATION.DTOs.Auth;

/// <summary>
/// Alteração de perfil, disponível apenas para administradores.
/// </summary>
public class AlterarRoleDTO
{
    [Required(ErrorMessage = "A role é obrigatória.")]
    public string Role { get; init; } = null!;
}
