using CRUD.PRODUTOS.DOMAIN.Common;

namespace CRUD.PRODUTOS.DOMAIN.Models;

public class Usuario : EntityBase
{
    public required string Login { get; set; }
    public required string SenhaHash { get; set; }
    public string Role { get; set; } = Roles.Padrao;
}
