using CRUD.PRODUTOS.DOMAIN.DTOs;
using CRUD.PRODUTOS.SERVICES;
using Microsoft.AspNetCore.Mvc;

namespace CRUD.PRODUTOS.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _auth;

    public AuthController(AuthService auth)
    {
        _auth = auth;
    }

    [HttpPost("registrar")]
    public async Task<IActionResult> Registrar(RegistrarUsuarioDTO dto)
    {
        await _auth.RegistrarAsync(dto);
        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDTO dto)
    {
        var token = await _auth.LoginAsync(dto);
        return Ok(new { token });
    }
}
