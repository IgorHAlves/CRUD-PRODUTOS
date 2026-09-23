using CRUD.PRODUTOS.DOMAIN.Security;

namespace CRUD.PRODUTOS.DATA.Security;

/// <summary>
/// Implementação de <see cref="IPasswordHasher"/> com BCrypt. Fica na camada de
/// infraestrutura para manter o domínio livre de dependências de criptografia.
/// </summary>
public class BCryptPasswordHasher : IPasswordHasher
{
    public string Hash(string senha) => BCrypt.Net.BCrypt.HashPassword(senha);

    public bool Verify(string senha, string hash) => BCrypt.Net.BCrypt.Verify(senha, hash);
}
