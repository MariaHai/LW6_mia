using AutoMapper;
using LW3.Documents;
using LW3.Models;
using LW3.Settings;
using MongoDB.Driver;

namespace LW3.Repositories;

public class UserRepository : MongoRepositoryBase<User, UserDocument>, IUserRepository
{
    public UserRepository(IMongoDatabase database, IMapper mapper)
        : base(database, mapper, CollectionNames.Users)
    {
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var document = await Collection.Find(x => x.Email == email).FirstOrDefaultAsync();
        return document is null ? null : ToModel(document);
    }

    public async Task<User?> GetByRefreshTokenHashAsync(string refreshTokenHash)
    {
        var document = await Collection.Find(x => x.RefreshTokenHash == refreshTokenHash).FirstOrDefaultAsync();
        return document is null ? null : ToModel(document);
    }
}
