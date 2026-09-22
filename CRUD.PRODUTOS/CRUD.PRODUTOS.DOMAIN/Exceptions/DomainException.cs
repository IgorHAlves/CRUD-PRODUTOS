namespace CRUD.PRODUTOS.DOMAIN.Exceptions;

/// <summary>
/// Base para as exceções previsíveis do domínio, traduzidas em respostas HTTP
/// pelo handler global da API.
/// </summary>
public abstract class DomainException : Exception
{
    protected DomainException(string mensagem) : base(mensagem)
    {
    }
}
