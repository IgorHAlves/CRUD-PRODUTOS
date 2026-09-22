namespace CRUD.PRODUTOS.DOMAIN.Repositories;

public interface IUnitOfWork
{
    Task CommitAsync(CancellationToken cancellationToken = default);
}
