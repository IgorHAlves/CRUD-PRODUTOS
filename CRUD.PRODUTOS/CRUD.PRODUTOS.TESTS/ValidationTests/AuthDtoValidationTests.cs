using System.ComponentModel.DataAnnotations;
using CRUD.PRODUTOS.APPLICATION.DTOs.Auth;
using Shouldly;
using Xunit;

namespace CRUD.PRODUTOS.TESTS.ValidationTests;

public class AuthDtoValidationTests
{
    private static IReadOnlyList<ValidationResult> Validar(object dto)
    {
        var resultados = new List<ValidationResult>();

        Validator.TryValidateObject(dto, new ValidationContext(dto), resultados, validateAllProperties: true);

        return resultados;
    }

    [Theory]
    [InlineData("")]
    [InlineData("1234567")]
    public void Should_Rejeitar_Senha_Curta(string senha)
    {
        var erros = Validar(new RegistrarUsuarioDTO { Login = "igor", Senha = senha });

        erros.ShouldNotBeEmpty();
    }

    [Fact]
    public void Should_Aceitar_Registro_Valido()
    {
        Validar(new RegistrarUsuarioDTO { Login = "igor", Senha = "senhaSegura1" }).ShouldBeEmpty();
    }

    [Fact]
    public void Should_Rejeitar_Login_Curto()
    {
        var erros = Validar(new RegistrarUsuarioDTO { Login = "ig", Senha = "senhaSegura1" });

        erros.ShouldContain(e => e.ErrorMessage == "O login deve ter entre 3 e 60 caracteres.");
    }

    [Fact]
    public void Should_Nao_Expor_Role_No_Registro()
    {
        // Garantia estrutural: o DTO de auto-cadastro não pode ter uma
        // propriedade Role, senão o cliente escolheria o próprio perfil.
        typeof(RegistrarUsuarioDTO).GetProperty("Role").ShouldBeNull();
    }
}
