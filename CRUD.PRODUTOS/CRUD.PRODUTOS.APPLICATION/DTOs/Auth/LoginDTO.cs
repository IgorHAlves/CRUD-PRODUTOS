using System.ComponentModel.DataAnnotations;

namespace CRUD.PRODUTOS.APPLICATION.DTOs.Auth;

public class LoginDTO
{
    [Required(ErrorMessage = "O login é obrigatório.")]
    public string Login { get; init; } = null!;

    [Required(ErrorMessage = "A senha é obrigatória.")]
    public string Senha { get; init; } = null!;
}
