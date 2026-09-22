using System.Reflection;
using Microsoft.OpenApi;

namespace CRUD.PRODUTOS.API.Extensions;

public static class SwaggerExtensions
{
    private const string EsquemaBearer = "Bearer";

    public static IServiceCollection AddSwaggerDocs(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "CRUD Produtos API", Version = "v1" });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            if (File.Exists(xmlPath))
                options.IncludeXmlComments(xmlPath);

            options.AddSecurityDefinition(EsquemaBearer, new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Digite apenas o token JWT (sem a palavra Bearer)"
            });

            // OpenAPI.NET v2+ referencia o esquema por um tipo dedicado, em vez
            // do par OpenApiReference/ReferenceType usado nas versões anteriores.
            options.AddSecurityRequirement(documento => new OpenApiSecurityRequirement
            {
                { new OpenApiSecuritySchemeReference(EsquemaBearer, documento), [] }
            });
        });

        return services;
    }
}
