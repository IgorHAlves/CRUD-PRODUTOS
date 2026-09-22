namespace CRUD.PRODUTOS.DOMAIN.Exceptions;

/// <summary>
/// Violação de uma regra de negócio. Traduzida em 400.
/// </summary>
public class RegraDeNegocioException : DomainException
{
    public RegraDeNegocioException(string mensagem) : base(mensagem)
    {
    }
}
