namespace CRUD.PRODUTOS.DOMAIN.Security;

/// <summary>
/// Abstrai o algoritmo de hash de senha para que o domínio não dependa
/// de uma biblioteca de criptografia específica.
/// </summary>
public interface IPasswordHasher
{
    string Hash(string senha);

    bool Verify(string senha, string hash);
}
