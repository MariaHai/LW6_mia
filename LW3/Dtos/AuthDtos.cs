using LW3.Models;

namespace LW3.Dtos;

// POST /api/auth/register
public class RegisterDto
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}

// POST /api/auth/login
public class LoginDto
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}

// POST /api/auth/refresh
public class RefreshTokenDto
{
    public string RefreshToken { get; set; } = "";
}

// Що повертає API після реєстрації (без хешу пароля і токенів)
public class UserResponseDto
{
    public string Id { get; set; } = "";
    public string Email { get; set; } = "";
    public UserRole Role { get; set; }
    public DateTime CreatedAt { get; set; }
}

// Пара токенів після входу або оновлення
public class AuthResponseDto
{
    public string AccessToken { get; set; } = "";
    public DateTime AccessTokenExpiresAt { get; set; }
    public string RefreshToken { get; set; } = "";
    public DateTime RefreshTokenExpiresAt { get; set; }
    public string TokenType { get; set; } = "Bearer";
    public string Email { get; set; } = "";
    public UserRole Role { get; set; }
}
