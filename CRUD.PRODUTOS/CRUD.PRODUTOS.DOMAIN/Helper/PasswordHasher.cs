namespace CRUD.PRODUTOS.DOMAIN.Helper;

public class PasswordHasher
{
    public static string Hash(string senha)
    {
        return BCrypt.Net.BCrypt.HashPassword(senha);   
    }

    public static bool Verify(string senha, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(senha, hash);
    }
}