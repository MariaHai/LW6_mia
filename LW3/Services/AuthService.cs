using AutoMapper;
using LW3.Dtos;
using LW3.Exceptions;
using LW3.Models;
using LW3.Repositories;

namespace LW3.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly ITokenService _tokens;
    private readonly IMapper _mapper;

    public AuthService(IUserRepository users, ITokenService tokens, IMapper mapper)
    {
        _users = users;
        _tokens = tokens;
        _mapper = mapper;
    }

    // ============ Реєстрація ============
    public async Task<UserResponseDto> RegisterAsync(RegisterDto dto)
    {
        var email = NormalizeEmail(dto.Email);

        if (await _users.GetByEmailAsync(email) is not null)
            throw new ConflictException($"Користувач з email {email} вже зареєстрований.");

        var user = new User
        {
            Email = email,
            // BCrypt: хеш із випадковою сіллю; сам пароль ніде не зберігається
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = UserRole.User, // роль Admin через реєстрацію отримати не можна
            CreatedAt = DateTime.UtcNow
        };

        var created = await _users.CreateAsync(user);
        return _mapper.Map<UserResponseDto>(created);
    }

    // ============ Вхід ============
    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _users.GetByEmailAsync(NormalizeEmail(dto.Email));

        // однакове повідомлення для "немає користувача" і "невірний пароль"
        if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new UnauthorizedException("Невірний email або пароль.");

        return await IssueTokensAsync(user);
    }

    // ============ Оновлення токенів ============
    public async Task<AuthResponseDto> RefreshAsync(RefreshTokenDto dto)
    {
        // токен шукається за хешем: так перевіряється, що він належить саме цьому користувачу
        var user = await _users.GetByRefreshTokenHashAsync(_tokens.HashToken(dto.RefreshToken))
            ?? throw new UnauthorizedException("Недійсний refresh token (уже використаний або не існує).");

        if (user.RefreshTokenExpiresAt is null || user.RefreshTokenExpiresAt <= DateTime.UtcNow)
            throw new UnauthorizedException("Строк дії refresh token минув. Увійдіть знову.");

        // ротація: старий токен замінюється новим і більше не діє
        return await IssueTokensAsync(user);
    }

    private async Task<AuthResponseDto> IssueTokensAsync(User user)
    {
        var (accessToken, accessExpires) = _tokens.GenerateAccessToken(user);
        var refreshToken = _tokens.GenerateRefreshToken();
        var refreshExpires = _tokens.GetRefreshTokenExpiry();

        user.RefreshTokenHash = _tokens.HashToken(refreshToken);
        user.RefreshTokenExpiresAt = refreshExpires;
        await _users.UpdateAsync(user.Id!, user);

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            AccessTokenExpiresAt = accessExpires,
            RefreshToken = refreshToken,
            RefreshTokenExpiresAt = refreshExpires,
            Email = user.Email,
            Role = user.Role
        };
    }

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();
}
