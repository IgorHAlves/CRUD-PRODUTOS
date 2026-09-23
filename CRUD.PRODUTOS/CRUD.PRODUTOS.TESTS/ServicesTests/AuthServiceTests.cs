using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CRUD.PRODUTOS.APPLICATION.Configuration;
using CRUD.PRODUTOS.APPLICATION.DTOs.Auth;
using CRUD.PRODUTOS.APPLICATION.Services;
using CRUD.PRODUTOS.DATA.Security;
using CRUD.PRODUTOS.DOMAIN.Common;
using CRUD.PRODUTOS.DOMAIN.Exceptions;
using CRUD.PRODUTOS.DOMAIN.Models;
using CRUD.PRODUTOS.DOMAIN.Repositories;
using CRUD.PRODUTOS.DOMAIN.Security;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;
using Xunit;

namespace CRUD.PRODUTOS.TESTS.ServicesTests;

public class AuthServiceTests
{
    private const string Senha = "senhaSegura1";

    private readonly Mock<IUsuarioRepository> _usuarioRepository = new(MockBehavior.Strict);
    private readonly Mock<IUnitOfWork> _unitOfWork = new(MockBehavior.Strict);
    private readonly IPasswordHasher _passwordHasher = new BCryptPasswordHasher();
    private readonly IAuthService _authService;

    public AuthServiceTests()
    {
        var jwtOptions = Options.Create(new JwtOptions
        {
            Key = "chave-de-teste-com-mais-de-32-caracteres!",
            Issuer = "CRUD.PRODUTOS.API",
            Audience = "CRUD.PRODUTOS.CLIENTS",
            ExpireMinutes = 60
        });

        _unitOfWork
            .Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _authService = new AuthService(
            jwtOptions,
            _usuarioRepository.Object,
            _unitOfWork.Object,
            _passwordHasher);
    }

    private Usuario UsuarioPersistido(string login = "igor", string role = Roles.Padrao) =>
        new()
        {
            Id = 1,
            Login = login,
            SenhaHash = _passwordHasher.Hash(Senha),
            Role = role
        };

    [Fact]
    public async Task Should_Registrar_Usuario_Com_Sucesso()
    {
        //Arrange
        Usuario? criado = null;

        _usuarioRepository
            .Setup(r => r.ExisteLoginAsync("igor", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _usuarioRepository
            .Setup(r => r.AdicionarAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()))
            .Callback<Usuario, CancellationToken>((u, _) => criado = u)
            .Returns(Task.CompletedTask);

        //Act
        await _authService.RegistrarAsync(new RegistrarUsuarioDTO { Login = "igor", Senha = Senha });

        //Assert
        criado.ShouldNotBeNull();
        criado.Login.ShouldBe("igor");
        criado.SenhaHash.ShouldNotBe(Senha, "a senha nunca pode ser gravada em texto puro");
        _passwordHasher.Verify(Senha, criado.SenhaHash).ShouldBeTrue();
        _unitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_Registrar_Sempre_Com_Role_Padrao()
    {
        //Arrange: mesmo que o cliente tente, o auto-cadastro não vira Admin
        Usuario? criado = null;

        _usuarioRepository
            .Setup(r => r.ExisteLoginAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _usuarioRepository
            .Setup(r => r.AdicionarAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()))
            .Callback<Usuario, CancellationToken>((u, _) => criado = u)
            .Returns(Task.CompletedTask);

        //Act
        await _authService.RegistrarAsync(new RegistrarUsuarioDTO { Login = "invasor", Senha = Senha });

        //Assert
        criado.ShouldNotBeNull();
        criado.Role.ShouldBe(Roles.Padrao);
    }

    [Fact]
    public async Task Should_Throw_Ao_Registrar_Login_Duplicado()
    {
        //Arrange
        _usuarioRepository
            .Setup(r => r.ExisteLoginAsync("igor", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        //Act
        var ex = await Should.ThrowAsync<RegraDeNegocioException>(
            () => _authService.RegistrarAsync(new RegistrarUsuarioDTO { Login = "igor", Senha = Senha }));

        //Assert
        ex.Message.ShouldBe("Login já cadastrado");
        _unitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Should_Realizar_Login_E_Gerar_Token()
    {
        //Arrange
        _usuarioRepository
            .Setup(r => r.ObterPorLoginAsync("igor", It.IsAny<CancellationToken>()))
            .ReturnsAsync(UsuarioPersistido(role: Roles.Admin));

        //Act
        var resposta = await _authService.LoginAsync(new LoginDTO { Login = "igor", Senha = Senha });

        //Assert
        resposta.Token.ShouldNotBeNullOrWhiteSpace();
        resposta.ExpiraEm.ShouldBeGreaterThan(DateTime.UtcNow);

        var token = new JwtSecurityTokenHandler().ReadJwtToken(resposta.Token);
        token.Issuer.ShouldBe("CRUD.PRODUTOS.API");
        token.Audiences.ShouldContain("CRUD.PRODUTOS.CLIENTS");
        token.Claims.ShouldContain(c => c.Type == ClaimTypes.Role && c.Value == Roles.Admin);
    }

    [Fact]
    public async Task Should_Respeitar_ExpireMinutes_Da_Configuracao()
    {
        //Arrange: antes, a expiração era fixa em 2h e ignorava a configuração
        _usuarioRepository
            .Setup(r => r.ObterPorLoginAsync("igor", It.IsAny<CancellationToken>()))
            .ReturnsAsync(UsuarioPersistido());

        //Act
        var resposta = await _authService.LoginAsync(new LoginDTO { Login = "igor", Senha = Senha });

        //Assert
        resposta.ExpiraEm.ShouldBe(DateTime.UtcNow.AddMinutes(60), TimeSpan.FromMinutes(1));
    }

    [Fact]
    public async Task Should_Throw_Login_Com_Senha_Incorreta()
    {
        //Arrange
        _usuarioRepository
            .Setup(r => r.ObterPorLoginAsync("igor", It.IsAny<CancellationToken>()))
            .ReturnsAsync(UsuarioPersistido());

        //Act + Assert
        await Should.ThrowAsync<CredenciaisInvalidasException>(
            () => _authService.LoginAsync(new LoginDTO { Login = "igor", Senha = "senhaErrada1" }));
    }

    [Fact]
    public async Task Should_Throw_Login_Com_Usuario_Inexistente()
    {
        //Arrange
        _usuarioRepository
            .Setup(r => r.ObterPorLoginAsync("fantasma", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Usuario?)null);

        //Act
        var ex = await Should.ThrowAsync<CredenciaisInvalidasException>(
            () => _authService.LoginAsync(new LoginDTO { Login = "fantasma", Senha = Senha }));

        //Assert: mensagem genérica, para não revelar quais logins existem
        ex.Message.ShouldBe("Login ou senha inválidos");
    }

    [Fact]
    public async Task Should_Alterar_Role_Do_Usuario()
    {
        //Arrange
        var usuario = UsuarioPersistido();

        _usuarioRepository
            .Setup(r => r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);

        //Act
        await _authService.AlterarRoleAsync(1, new AlterarRoleDTO { Role = Roles.Admin });

        //Assert
        usuario.Role.ShouldBe(Roles.Admin);
        _unitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_Throw_Ao_Alterar_Para_Role_Invalida()
    {
        //Act + Assert
        await Should.ThrowAsync<RegraDeNegocioException>(
            () => _authService.AlterarRoleAsync(1, new AlterarRoleDTO { Role = "SuperAdmin" }));

        _unitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Should_Throw_Ao_Alterar_Role_De_Usuario_Inexistente()
    {
        //Arrange
        _usuarioRepository
            .Setup(r => r.ObterPorIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Usuario?)null);

        //Act + Assert
        await Should.ThrowAsync<NaoEncontradoException>(
            () => _authService.AlterarRoleAsync(99, new AlterarRoleDTO { Role = Roles.Admin }));
    }
}
