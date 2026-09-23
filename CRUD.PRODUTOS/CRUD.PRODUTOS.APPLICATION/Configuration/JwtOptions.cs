using System.ComponentModel.DataAnnotations;

namespace CRUD.PRODUTOS.APPLICATION.Configuration;

/// <summary>
/// Configuração do JWT, validada na inicialização da aplicação.
/// </summary>
public class JwtOptions
{
    public const string SectionName = "Jwt";

    /// <summary>Chave simétrica de assinatura. Nunca versionar: use user-secrets ou variável de ambiente.</summary>
    [Required(ErrorMessage = "Jwt:Key não configurada. Use 'dotnet user-secrets set \"Jwt:Key\" \"<chave>\"'.")]
    [MinLength(32, ErrorMessage = "Jwt:Key deve ter no mínimo 32 caracteres.")]
    public string Key { get; init; } = string.Empty;

    [Required]
    public string Issuer { get; init; } = string.Empty;

    [Required]
    public string Audience { get; init; } = string.Empty;

    [Range(1, 1440)]
    public int ExpireMinutes { get; init; } = 60;
}
