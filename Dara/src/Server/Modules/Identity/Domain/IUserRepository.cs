using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Identity.Domain;

public interface IUserRepository : IRepository
{
    public Task<User?> GetByUserIdentifierAsync(string userIdentifier);
}