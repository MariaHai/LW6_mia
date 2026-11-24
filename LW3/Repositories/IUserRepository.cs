using LW3.Models;

namespace LW3.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByRefreshTokenHashAsync(string refreshTokenHash);
}
