namespace CRUD.PRODUTOS.DOMAIN.Exceptions;

/// <summary>
/// Login ou senha inválidos. Traduzida em 401, sempre com mensagem genérica
/// para não revelar quais logins existem.
/// </summary>
public class CredenciaisInvalidasException : DomainException
{
    public CredenciaisInvalidasException() : base("Login ou senha inválidos")
    {
    }
}
