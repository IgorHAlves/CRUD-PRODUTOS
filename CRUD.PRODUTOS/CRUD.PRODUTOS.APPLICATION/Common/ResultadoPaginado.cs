namespace CRUD.PRODUTOS.APPLICATION.Common;

/// <summary>
/// Página de resultados devolvida pelos serviços de consulta.
/// </summary>
public class ResultadoPaginado<T>
{
    public IReadOnlyList<T> Itens { get; init; } = [];
    public int TotalItens { get; init; }
    public int PaginaAtual { get; init; }
    public int TotalPaginas { get; init; }

    public static ResultadoPaginado<T> Criar(IReadOnlyList<T> itens, int totalItens, int page, int limit) =>
        new()
        {
            Itens = itens,
            TotalItens = totalItens,
            PaginaAtual = page,
            TotalPaginas = limit <= 0 ? 0 : (int)Math.Ceiling(totalItens / (double)limit)
        };
}
