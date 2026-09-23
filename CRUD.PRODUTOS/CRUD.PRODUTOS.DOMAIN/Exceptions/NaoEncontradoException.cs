namespace CRUD.PRODUTOS.DOMAIN.Exceptions;

/// <summary>
/// Recurso solicitado não existe. Traduzida em 404.
/// </summary>
public class NaoEncontradoException : DomainException
{
    public NaoEncontradoException(string mensagem) : base(mensagem)
    {
    }

    public static NaoEncontradoException Produto(int id) =>
        new($"Produto {id} não encontrado");
}
