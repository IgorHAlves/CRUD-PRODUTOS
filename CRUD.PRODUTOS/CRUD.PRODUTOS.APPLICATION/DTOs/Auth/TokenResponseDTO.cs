namespace CRUD.PRODUTOS.APPLICATION.DTOs.Auth;

public class TokenResponseDTO
{
    public required string Token { get; init; }
    public required DateTime ExpiraEm { get; init; }
}
