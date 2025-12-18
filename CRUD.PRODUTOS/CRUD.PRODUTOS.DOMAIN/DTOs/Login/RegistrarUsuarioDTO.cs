namespace CRUD.PRODUTOS.DOMAIN.DTOs;

public class RegistrarUsuarioDTO
{
    public string Login { get; set; } = null!;
    public string Senha { get; set; } = null!;
    public string Role { get; set; } = "Padrao"; 
}