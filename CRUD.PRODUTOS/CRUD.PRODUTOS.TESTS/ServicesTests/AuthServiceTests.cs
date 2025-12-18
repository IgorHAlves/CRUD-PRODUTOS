using CRUD.PRODUTOS.DATA.Data;
using CRUD.PRODUTOS.DATA.Repositories;
using CRUD.PRODUTOS.DOMAIN.DTOs;
using CRUD.PRODUTOS.SERVICES;
using Microsoft.Extensions.Configuration;
using Moq;
using Shouldly;
using Xunit;

namespace CRUD.PRODUTOS.TESTS.ServicesTests;

public class AuthServiceTests
{
    private readonly AppDBContext _dbContext;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _dbContext = TestAppDbContextFactory.Create();

        var usuarioRepository = new UsuarioRepository(_dbContext);
        var unitOfWork = new UnitOfWork(_dbContext);

        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(c => c["Jwt:Key"]).Returns("ChaveMestraSuperSecretaComMaisDe32Caracteres");
        mockConfig.Setup(c => c["Jwt:Issuer"]).Returns("CRUD.PRODUTOS.API");
        mockConfig.Setup(c => c["Jwt:Audience"]).Returns("CRUD.PRODUTOS.USUARIOS");

        _authService = new AuthService(mockConfig.Object, usuarioRepository, unitOfWork);
    }

    [Fact]
    public async Task Should_Registrar_Usuario_Com_Sucesso()
    {
        // Arrange
        var registrarDto = new RegistrarUsuarioDTO
        {
            Login = "igor.alves",
            Senha = "Senha@123",
            Role = "Admin"
        };

        // Act
        await _authService.RegistrarAsync(registrarDto);

        // Assert
        var usuarioCriado = _dbContext.Usuarios.FirstOrDefault(u => u.Login == "igor.alves");
        usuarioCriado.ShouldNotBeNull();
        usuarioCriado.Role.ShouldBe("Admin");
        usuarioCriado.SenhaHash.ShouldNotBe(registrarDto.Senha);
    }

    [Fact]
    public async Task Should_Throw_Ao_Registrar_Login_Duplicado()
    {
        // Arrange
        var registrarDto = new RegistrarUsuarioDTO { Login = "duplicado", Senha = "123", Role = "Common" };
        await _authService.RegistrarAsync(registrarDto);

        // Act
        var ex = await Should.ThrowAsync<ArgumentException>(async () =>
        {
            await _authService.RegistrarAsync(registrarDto);
        });

        // Assert
        ex.Message.ShouldBe("Login já existe");
    }

    [Fact]
    public async Task Should_Realizar_Login_E_Gerar_Token()
    {
        // Arrange
        var login = "usuario.teste";
        var senha = "Password123";
        await _authService.RegistrarAsync(new RegistrarUsuarioDTO 
        { 
            Login = login, 
            Senha = senha, 
            Role = "Admin" 
        });

        var loginDto = new LoginDTO { Login = login, Senha = senha };

        // Act
        var token = await _authService.LoginAsync(loginDto);

        // Assert
        token.ShouldNotBeNullOrEmpty();
        token.Split('.').Length.ShouldBe(3); // Um JWT válido tem 3 partes (header, payload, signature)
    }

    [Fact]
    public async Task Should_Throw_Login_Com_Senha_Incorreta()
    {
        // Arrange
        await _authService.RegistrarAsync(new RegistrarUsuarioDTO 
        { 
            Login = "usuario.errado", 
            Senha = "SenhaCorreta", 
            Role = "Common" 
        });

        var loginDto = new LoginDTO { Login = "usuario.errado", Senha = "SenhaErrada" };

        // Act e Assert
        var ex = await Should.ThrowAsync<ArgumentException>(async () =>
        {
            await _authService.LoginAsync(loginDto);
        });

        ex.Message.ShouldBe("Login ou senha inválidos");
    }

    [Fact]
    public async Task Should_Throw_Login_Com_Usuario_Inexistente()
    {
        // Arrange
        var loginDto = new LoginDTO { Login = "naoexiste", Senha = "123" };

        // Act e Assert
        var ex = await Should.ThrowAsync<ArgumentException>(async () =>
        {
            await _authService.LoginAsync(loginDto);
        });

        ex.Message.ShouldBe("Login ou senha inválidos");
    }
}