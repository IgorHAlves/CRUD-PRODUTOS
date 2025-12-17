using CRUD.PRODUTOS.DOMAIN.Models;
using Shouldly;
using Xunit;

namespace CRUD.PRODUTOS.TESTS.ModelsTests;

public class EntityBaseTests
{
    [Fact]
    public void Should_Atualizar_Data_Alteracao()
    {
        // Arrange
        var entity = new EntityBase();
        var antes = DateTime.UtcNow;

        // Act
        entity.AtualizarDataAlteracao();

        var depois = DateTime.UtcNow;

        // Assert
        entity.DataAlteracao.ShouldBeGreaterThanOrEqualTo(antes);
        entity.DataAlteracao.ShouldBeLessThanOrEqualTo(depois);
    }
}