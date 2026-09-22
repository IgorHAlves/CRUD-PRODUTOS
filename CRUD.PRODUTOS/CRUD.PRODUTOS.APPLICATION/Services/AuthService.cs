using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CRUD.PRODUTOS.APPLICATION.Configuration;
using CRUD.PRODUTOS.APPLICATION.DTOs.Auth;
using CRUD.PRODUTOS.DOMAIN.Common;
using CRUD.PRODUTOS.DOMAIN.Exceptions;
using CRUD.PRODUTOS.DOMAIN.Models;
using CRUD.PRODUTOS.DOMAIN.Repositories;
using CRUD.PRODUTOS.DOMAIN.Security;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CRUD.PRODUTOS.APPLICATION.Services;

public class AuthService : IAuthService
{
    private readonly JwtOptions _jwtOptions;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(
        IOptions<JwtOptions> jwtOptions,
        IUsuarioRepository usuarioRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher)
    {
        _jwtOptions = jwtOptions.Value;
        _usuarioRepository = usuarioRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task RegistrarAsync(RegistrarUsuarioDTO dto, CancellationToken cancellationToken = default)
    {
        var login = dto.Login.Trim();

        if (await _usuarioRepository.ExisteLoginAsync(login, cancellationToken))
            throw new RegraDeNegocioException("Login já cadastrado");

        var usuario = new Usuario
        {
            Login = login,
            SenhaHash = _passwordHasher.Hash(dto.Senha),
            // O perfil nunca vem do cliente: auto-cadastro sempre gera usuário comum.
            Role = Roles.Padrao
        };

        await _usuarioRepository.AdicionarAsync(usuario, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }

    public async Task<TokenResponseDTO> LoginAsync(LoginDTO dto, CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarioRepository.ObterPorLoginAsync(dto.Login.Trim(), cancellationToken)
                      ?? throw new CredenciaisInvalidasException();

        if (!_passwordHasher.Verify(dto.Senha, usuario.SenhaHash))
            throw new CredenciaisInvalidasException();

        return GerarToken(usuario);
    }

    public async Task AlterarRoleAsync(int usuarioId, AlterarRoleDTO dto, CancellationToken cancellationToken = default)
    {
        if (!Roles.EhValida(dto.Role))
            throw new RegraDeNegocioException($"Role inválida. Valores aceitos: {Roles.Admin}, {Roles.Padrao}");

        var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId, cancellationToken)
                      ?? throw new NaoEncontradoException($"Usuário {usuarioId} não encontrado");

        usuario.Role = dto.Role;

        await _unitOfWork.CommitAsync(cancellationToken);
    }

    private TokenResponseDTO GerarToken(Usuario usuario)
    {
        var expiraEm = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpireMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name, usuario.Login),
            new(ClaimTypes.Role, usuario.Role)
        };

        var credenciais = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: expiraEm,
            signingCredentials: credenciais);

        return new TokenResponseDTO
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiraEm = expiraEm
        };
    }
}
