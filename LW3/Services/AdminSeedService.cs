using LW3.Models;
using LW3.Repositories;
using LW3.Settings;
using Microsoft.Extensions.Options;

namespace LW3.Services;

// Створює початкового адміністратора з секції AdminSeed в appsettings.json.
// Працює у фоні, тож не затримує запуск застосунку, навіть якщо база недоступна.
public class AdminSeedService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly AdminSeedSettings _settings;
    private readonly ILogger<AdminSeedService> _logger;

    public AdminSeedService(
        IServiceProvider services,
        IOptions<AdminSeedSettings> settings,
        ILogger<AdminSeedService> logger)
    {
        _services = services;
        _settings = settings.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (string.IsNullOrWhiteSpace(_settings.Email) || string.IsNullOrWhiteSpace(_settings.Password))
            return;

        try
        {
            using var scope = _services.CreateScope();
            var users = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            var email = _settings.Email.Trim().ToLowerInvariant();

            var existing = await users.GetByEmailAsync(email);
            if (existing is null)
            {
                await users.CreateAsync(new User
                {
                    Email = email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(_settings.Password),
                    Role = UserRole.Admin,
                    CreatedAt = DateTime.UtcNow
                });
                _logger.LogInformation("Створено початкового адміністратора {Email}.", email);
            }
            else if (existing.Role != UserRole.Admin)
            {
                existing.Role = UserRole.Admin;
                await users.UpdateAsync(existing.Id!, existing);
                _logger.LogInformation("Користувачу {Email} призначено роль Admin.", email);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Не вдалося створити початкового адміністратора.");
        }
    }
}
