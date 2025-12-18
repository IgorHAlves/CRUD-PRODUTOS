using CRUD.PRODUTOS.DATA.Data;
using CRUD.PRODUTOS.DATA.Repositories;
using CRUD.PRODUTOS.DOMAIN.Models;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace CRUD.PRODUTOS.TESTS.RepositoriesTests;

public class UsuarioRepositoryTests
{
    private readonly AppDBContext _context;
    private readonly UsuarioRepository _repository;

    public UsuarioRepositoryTests()
    {
        // Usa o seu Factory para banco em memória
        _context = TestAppDbContextFactory.Create();
        _repository = new UsuarioRepository(_context);
    }

    [Fact]
    public async Task Should_Criar_Usuario()
    {
        // Arrange
        var usuario = new Usuario
        {
            Login = "igor.alves",
            SenhaHash = "hash_secreto_123",
            Role = "Admin",
            DataCriacao = DateTime.UtcNow
        };

        // Act
        await _repository.CriarAsync(usuario);
        await _context.SaveChangesAsync();

        // Assert
        var salvo = await _context.Usuarios.FirstOrDefaultAsync(u => u.Login == "igor.alves");
        
        salvo.ShouldNotBeNull();
        salvo.Login.ShouldBe("igor.alves");
        salvo.Role.ShouldBe("Admin");
    }

    [Fact]
    public async Task Should_Obter_Usuario_Por_Login()
    {
        // Arrange
        var loginProcurado = "usuario.teste";
        _context.Usuarios.Add(new Usuario
        {
            Login = loginProcurado,
            SenhaHash = "hash123",
            Role = "Common"
        });
        await _context.SaveChangesAsync();

        // Act
        var usuario = await _repository.ObterPorLoginAsync(loginProcurado);

        // Assert
        usuario.ShouldNotBeNull();
        usuario.Login.ShouldBe(loginProcurado);
    }

    [Fact]
    public async Task Should_Retornar_Null_Quando_Login_Nao_Existe()
    {
        // Act
        var usuario = await _repository.ObterPorLoginAsync("login.inexistente");

        // Assert
        usuario.ShouldBeNull();
    }

    [Fact]
    public async Task Should_Respeitar_Unicidade_De_Login()
    {
        // Arrange
        var usuario1 = new Usuario { Login = "admin", SenhaHash = "123", Role = "Admin" };
        var usuario2 = new Usuario { Login = "admin", SenhaHash = "456", Role = "Padrao" };

        await _repository.CriarAsync(usuario1);
        await _context.SaveChangesAsync();

        // Act e Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
        {
            await _repository.CriarAsync(usuario2);
            await _context.SaveChangesAsync();
        });
    }
}