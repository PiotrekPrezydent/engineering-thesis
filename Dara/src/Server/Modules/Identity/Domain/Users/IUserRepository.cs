using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Identity.Domain.Users;

public interface IUserRepository : IRepository
{
    public Task AddAsync(User user);

    public Task<User> GetByIdAsync(UserId id);

    // the account whose open registration code has this hash
    public Task<User?> FindByClientRegistrationCodeHashAsync(string codeHash);

    // the account one of whose clients has an open login code with this hash
    public Task<User?> FindByClientLoginCodeHashAsync(string codeHash);

    // the login is normalized by the repository, null when no account has it
    public Task<User?> FindByLoginAsync(string login);
}
