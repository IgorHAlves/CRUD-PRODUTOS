using CRUD.PRODUTOS.INTERFACES;

namespace CRUD.PRODUTOS.DATA.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDBContext _dbContext;

    public UnitOfWork(AppDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CommitAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}