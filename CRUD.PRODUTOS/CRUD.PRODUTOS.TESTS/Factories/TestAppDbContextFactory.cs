using CRUD.PRODUTOS.DATA.Data;
using Microsoft.EntityFrameworkCore;

namespace CRUD.PRODUTOS.TESTS.Factories;

public static class TestAppDbContextFactory
{
    /// <summary>
    /// Contexto isolado por teste. O provider InMemory não traduz SQL real —
    /// serve para exercitar o mapeamento e o change tracker, não para validar
    /// as consultas específicas do Postgres.
    /// </summary>
    public static AppDBContext Create()
    {
        var options = new DbContextOptionsBuilder<AppDBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging()
            .Options;

        return new AppDBContext(options);
    }
}
