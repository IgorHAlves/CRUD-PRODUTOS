namespace CRUD.PRODUTOS.DOMAIN.Models;

public class Usuario : EntityBase
{
    public string Login { get; set; } = null!;
    public string SenhaHash { get; set; } = null!;
    public string Role { get; set; } = "Padrao";
}

