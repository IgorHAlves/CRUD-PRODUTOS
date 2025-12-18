namespace CRUD.PRODUTOS.DOMAIN.DTOs;

public class TokenResponseDTO
{
    public string Token { get; set; }
    public DateTime ExpiraEm { get; set; }
}