using LW3.Models;

namespace LW3.Services;

public interface ITokenService
{
    // access token (JWT) з claims: NameIdentifier, Email, Role
    (string Token, DateTime ExpiresAt) GenerateAccessToken(User user);

    // випадковий refresh token (у відкритому вигляді віддається клієнту)
    string GenerateRefreshToken();

    DateTime GetRefreshTokenExpiry();

    // SHA-256 хеш токена: у базі зберігається лише він
    string HashToken(string token);
}
