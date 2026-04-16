using Dara.BuildingBlocks.Domain.Business;

namespace Dara.Modules.AccessManagment.Domain.Users;

public interface IUserRepository : IRepository<User>
{
    public Task<User> FindByEmail(UserEmail email);
    
    public Task<User> FindByNickname(UserNickname nickname);
    
    public Task<User> FindById(Guid id);
    
    public Task Add(User user);
    
    public Task Save(User user);
}