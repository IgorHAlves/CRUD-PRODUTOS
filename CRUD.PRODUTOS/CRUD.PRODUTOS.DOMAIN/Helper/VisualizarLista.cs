namespace CRUD.PRODUTOS.DOMAIN.Helper;

public class VisualizarLista <TEntity>
{
    public virtual IList<TEntity> Itens { get; set; } = new List<TEntity>();
    public int TotalItens { get; set; }
    public int PaginaAtual { get; set; }
    public int TotalPaginas { get; set; }
}