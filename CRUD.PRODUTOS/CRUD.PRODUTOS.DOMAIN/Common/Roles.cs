namespace CRUD.PRODUTOS.DOMAIN.Common;

/// <summary>
/// Perfis de acesso reconhecidos pela aplicação.
/// </summary>
public static class Roles
{
    public const string Admin = "Admin";
    public const string Padrao = "Padrao";

    public static bool EhValida(string role) =>
        role is Admin or Padrao;
}
