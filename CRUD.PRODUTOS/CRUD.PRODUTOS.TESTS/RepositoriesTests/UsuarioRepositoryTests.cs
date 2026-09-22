using CRUD.PRODUTOS.DATA.Data;
using CRUD.PRODUTOS.DATA.Repositories;
using CRUD.PRODUTOS.DOMAIN.Common;
using CRUD.PRODUTOS.DOMAIN.Models;
using CRUD.PRODUTOS.DOMAIN.Repositories;
using CRUD.PRODUTOS.TESTS.Factories;
using Shouldly;
using Xunit;

namespace CRUD.PRODUTOS.TESTS.RepositoriesTests;

public class UsuarioRepositoryTests : IDisposable
{
    private readonly AppDBContext _dbContext;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UsuarioRepositoryTests()
    {
        _dbContext = TestAppDbContextFactory.Create();
        _usuarioRepository = new UsuarioRepository(_dbContext);
        _unitOfWork = new UnitOfWork(_dbContext, TimeProvider.System);
    }

    public void Dispose() => _dbContext.Dispose();

    private async Task<Usuario> SemearAsync(string login = "igor")
    {
        var usuario = new Usuario
        {
            Login = login,
            SenhaHash = "hash-ficticio",
            Role = Roles.Padrao
        };

        await _usuarioRepository.AdicionarAsync(usuario);
        await _unitOfWork.CommitAsync();

        return usuario;
    }

    [Fact]
    public async Task Should_Criar_Usuario()
    {
        //Act
        var usuario = await SemearAsync();

        //Assert
        usuario.Id.ShouldBeGreaterThan(0);
        usuario.DataCriacao.Kind.ShouldBe(DateTimeKind.Utc);
    }

    [Fact]
    public async Task Should_Obter_Usuario_Por_Login()
    {
        //Arrange
        await SemearAsync();

        //Act
        var usuario = await _usuarioRepository.ObterPorLoginAsync("igor");

        //Assert
        usuario.ShouldNotBeNull();
        usuario.Login.ShouldBe("igor");
        usuario.Role.ShouldBe(Roles.Padrao);
    }

    [Fact]
    public async Task Should_Obter_Usuario_Por_Id()
    {
        //Arrange
        var criado = await SemearAsync();

        //Act
        var usuario = await _usuarioRepository.ObterPorIdAsync(criado.Id);

        //Assert
        usuario.ShouldNotBeNull();
        usuario.Login.ShouldBe("igor");
    }

    [Fact]
    public async Task Should_Retornar_Null_Quando_Login_Nao_Existe()
    {
        (await _usuarioRepository.ObterPorLoginAsync("fantasma")).ShouldBeNull();
    }

    [Fact]
    public async Task Should_Indicar_Que_Login_Ja_Existe()
    {
        //Arrange
        await SemearAsync();

        //Act + Assert
        (await _usuarioRepository.ExisteLoginAsync("igor")).ShouldBeTrue();
        (await _usuarioRepository.ExisteLoginAsync("outro")).ShouldBeFalse();
    }
}
