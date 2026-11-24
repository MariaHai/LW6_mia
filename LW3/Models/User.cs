namespace LW3.Models;

public class User
{
    public string? Id { get; set; }
    public string Email { get; set; } = "";

    // лише криптографічний хеш (BCrypt із сіллю), пароль у відкритому вигляді не зберігається
    public string PasswordHash { get; set; } = "";

    public UserRole Role { get; set; } = UserRole.User;

    // SHA-256 хеш чинного refresh token і строк його дії
    public string? RefreshTokenHash { get; set; }
    public DateTime? RefreshTokenExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
