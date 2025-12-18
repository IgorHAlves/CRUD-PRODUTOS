using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CRUD.PRODUTOS.DOMAIN.DTOs;
using CRUD.PRODUTOS.DOMAIN.Helper;
using CRUD.PRODUTOS.DOMAIN.Models;
using CRUD.PRODUTOS.INTERFACES;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CRUD.PRODUTOS.SERVICES;

public class AuthService
{
    private readonly IConfiguration _config;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(IConfiguration config, IUsuarioRepository usuarioRepository,IUnitOfWork unitOfWork)
    {
        _config = config;
        _usuarioRepository = usuarioRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task RegistrarAsync(RegistrarUsuarioDTO dto)
    {
        if (await _usuarioRepository.ObterPorLoginAsync(dto.Login) != null)
            throw new ArgumentException("Login já existe");

        var usuario = new Usuario
        {
            Login = dto.Login,
            SenhaHash = PasswordHasher.Hash(dto.Senha),
            Role =  dto.Role,
            DataCriacao = DateTime.UtcNow
            
        };

        await _usuarioRepository.CriarAsync(usuario);
        await _unitOfWork.CommitAsync();
    }

    public async Task<string> LoginAsync(LoginDTO dto)
    {
        var usuario = await _usuarioRepository.ObterPorLoginAsync(dto.Login)
                      ?? throw new ArgumentException("Login ou senha inválidos");

        if (!PasswordHasher.Verify(dto.Senha, usuario.SenhaHash))
            throw new ArgumentException("Login ou senha inválidos");

        return GerarToken(usuario);
    }
    
    private string GerarToken(Usuario usuario)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, usuario.Login),
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            
            new Claim(ClaimTypes.Role, usuario.Role)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
        );

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
