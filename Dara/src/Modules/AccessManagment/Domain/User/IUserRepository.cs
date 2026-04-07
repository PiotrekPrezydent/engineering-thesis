using Dara.Core.Domain.Business;

namespace Dara.Modules.AccessManagment.Domain.User;

public interface IUserRepository : IRepository<User>
{
    public Task<User> GetUserByEmail(string email);
    
    public Task<User> GetUserById(Guid userId);
    
    public Task Save(User user);
    
    public Task Add(User user);
}