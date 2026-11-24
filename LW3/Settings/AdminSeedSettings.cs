namespace LW3.Settings;

// Початковий адміністратор: створюється при старті застосунку, якщо вказано Email і Password.
public class AdminSeedSettings
{
    public const string SectionName = "AdminSeed";

    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
