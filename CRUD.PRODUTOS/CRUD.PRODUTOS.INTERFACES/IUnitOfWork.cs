namespace CRUD.PRODUTOS.INTERFACES;

public interface IUnitOfWork
{
    Task CommitAsync();
}