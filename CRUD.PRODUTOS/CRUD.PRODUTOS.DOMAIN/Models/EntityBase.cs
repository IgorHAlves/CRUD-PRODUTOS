namespace CRUD.PRODUTOS.DOMAIN.Models;

public abstract class EntityBase
{
    public int Id { get; set; }

    /// <summary>
    /// Preenchida automaticamente pelo <c>UnitOfWork</c> ao persistir.
    /// </summary>
    public DateTime DataCriacao { get; set; }

    /// <summary>
    /// Preenchida automaticamente pelo <c>UnitOfWork</c> a cada alteração.
    /// </summary>
    public DateTime DataAlteracao { get; set; }
}
