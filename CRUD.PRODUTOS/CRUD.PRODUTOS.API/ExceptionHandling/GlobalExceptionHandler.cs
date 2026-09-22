using CRUD.PRODUTOS.DOMAIN.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CRUD.PRODUTOS.API.ExceptionHandling;

/// <summary>
/// Traduz as exceções de domínio em respostas ProblemDetails (RFC 7807),
/// dispensando os try/catch repetidos nos controllers.
/// </summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(
        IProblemDetailsService problemDetailsService,
        ILogger<GlobalExceptionHandler> logger)
    {
        _problemDetailsService = problemDetailsService;
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (status, titulo, detalhe) = Mapear(exception);

        if (status == StatusCodes.Status500InternalServerError)
        {
            // Erros inesperados são registrados por inteiro, mas o detalhe
            // técnico nunca vai para o cliente.
            _logger.LogError(exception, "Erro não tratado ao processar {Metodo} {Caminho}",
                httpContext.Request.Method, httpContext.Request.Path);
        }
        else
        {
            _logger.LogInformation("Requisição rejeitada ({Status}): {Mensagem}", status, exception.Message);
        }

        httpContext.Response.StatusCode = status;

        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = status,
                Title = titulo,
                Detail = detalhe,
                Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
            }
        });
    }

    private static (int Status, string Titulo, string Detalhe) Mapear(Exception exception) => exception switch
    {
        NaoEncontradoException ex =>
            (StatusCodes.Status404NotFound, "Recurso não encontrado", ex.Message),

        CredenciaisInvalidasException ex =>
            (StatusCodes.Status401Unauthorized, "Não autorizado", ex.Message),

        RegraDeNegocioException ex =>
            (StatusCodes.Status400BadRequest, "Requisição inválida", ex.Message),

        _ => (StatusCodes.Status500InternalServerError, "Erro interno",
            "Ocorreu um erro inesperado ao processar a requisição.")
    };
}
