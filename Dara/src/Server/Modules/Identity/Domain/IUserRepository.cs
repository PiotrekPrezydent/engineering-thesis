using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Identity.Domain;

public interface IUserRepository : IRepository
{
    public Task AddAsync(User user);
    
    public Task<User> GetByIdAsync(UserId id);
}