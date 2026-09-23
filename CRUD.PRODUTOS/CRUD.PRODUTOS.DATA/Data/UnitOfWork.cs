using CRUD.PRODUTOS.DOMAIN.Models;
using CRUD.PRODUTOS.DOMAIN.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CRUD.PRODUTOS.DATA.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDBContext _dbContext;
    private readonly TimeProvider _timeProvider;

    public UnitOfWork(AppDBContext dbContext, TimeProvider timeProvider)
    {
        _dbContext = dbContext;
        _timeProvider = timeProvider;
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        AplicarDatasDeAuditoria();

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Preenche DataCriacao/DataAlteracao de forma centralizada. Sem isso as
    /// colunas "timestamp with time zone" recebem DateTime.MinValue (Kind
    /// Unspecified), que o Npgsql rejeita em tempo de execução.
    /// </summary>
    private void AplicarDatasDeAuditoria()
    {
        var agora = _timeProvider.GetUtcNow().UtcDateTime;

        foreach (var entry in _dbContext.ChangeTracker.Entries<EntityBase>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.DataCriacao = agora;
                    entry.Entity.DataAlteracao = agora;
                    break;

                case EntityState.Modified:
                    entry.Property(e => e.DataCriacao).IsModified = false;
                    entry.Entity.DataAlteracao = agora;
                    break;
            }
        }
    }
}
