using CRUD.PRODUTOS.APPLICATION.Configuration;
using CRUD.PRODUTOS.APPLICATION.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CRUD.PRODUTOS.APPLICATION;

public static class DependencyInjection
{
    /// <summary>
    /// Registra os serviços de aplicação e valida a configuração do JWT na
    /// inicialização — a aplicação não sobe com uma chave ausente ou fraca.
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddScoped<IProdutoService, ProdutoService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
