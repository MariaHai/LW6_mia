namespace LW3.Settings;

public class JwtSettings
{
    public const string SectionName = "Jwt";

    // секретний ключ підпису (не менше 32 символів); краще зберігати в User Secrets
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;

    // тривалість дії access token, хвилин
    public int AccessTokenMinutes { get; set; } = 15;

    // тривалість дії refresh token, днів
    public int RefreshTokenDays { get; set; } = 7;
}
