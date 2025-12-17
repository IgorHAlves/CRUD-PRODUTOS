using CRUD.PRODUTOS.DATA.Data;
using Microsoft.EntityFrameworkCore;

namespace CRUD.PRODUTOS.TESTS;

public class TestAppDbContextFactory
{
    public static AppDBContext Create()
    {
        var options = new DbContextOptionsBuilder<AppDBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging()
            .Options;

        return new AppDBContext(options);
    }
}