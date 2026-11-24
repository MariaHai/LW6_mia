using System.Security.Claims;
using LW3.Dtos;
using LW3.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LW3.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;

    public AuthController(IAuthService service)
    {
        _service = service;
    }

    // Реєстрація: створює користувача з роллю User, пароль зберігається лише як хеш
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<UserResponseDto>> Register(RegisterDto dto)
    {
        var user = await _service.RegisterAsync(dto);
        return StatusCode(StatusCodes.Status201Created, user);
    }

    // Вхід: перевіряє облікові дані й видає пару access/refresh token
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto) =>
        Ok(await _service.LoginAsync(dto));

    // Оновлення: перевіряє refresh token і видає нову пару
    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponseDto>> Refresh(RefreshTokenDto dto) =>
        Ok(await _service.RefreshAsync(dto));

    // Допоміжний endpoint для перевірки токена: повертає claims поточного користувача
    [Authorize]
    [HttpGet("me")]
    public IActionResult Me() => Ok(new
    {
        Id = User.FindFirstValue(ClaimTypes.NameIdentifier),
        Email = User.FindFirstValue(ClaimTypes.Email),
        Role = User.FindFirstValue(ClaimTypes.Role)
    });
}
