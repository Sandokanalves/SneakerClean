using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SneakerClean.Application.DTOs;
using SneakerClean.Application.Interfaces;

namespace SneakerClean.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ValidatedControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Autentica um usuário e retorna o token JWT.
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var validation = await ValidateRequestAsync(request);
        if (validation != null) return validation;

        var result = await _userService.AuthenticateAsync(request);

        if (result == null)
            return Unauthorized(new { message = "E-mail ou senha inválidos." });

        return Ok(result);
    }
}