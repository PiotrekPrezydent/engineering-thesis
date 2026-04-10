using Dara.Modules.AccessManagment.Domain.Exceptions;
using Dara.Modules.AccessManagment.Domain.User;

namespace Dara.Modules.AccessManagment.Infrastructure;

public class UserRepository : IUserRepository
{
    private List<User> _users;

    public UserRepository()
    {
        _users = new();
    }
    public Task<User> GetUserByEmail(UserEmail email)
    {
        User? user = _users.Find(e=>e.Email.Equals(email));
        if (user == null)
            throw new UserNotFoundException();
        
        return Task.FromResult(user);
    }

    public Task<User> GetUserById(Guid userId)
    {
        User? user = _users.Find(e=>e.Id.Equals(userId));
        
        if (user == null)
            throw new UserNotFoundException();
        
        return Task.FromResult(user);
    }

    public Task Save(User user)
    {
        User? repoUser = _users.Find(e=>e.Id.Equals(user.Id));
        
        if (user == null)
            throw new UserNotFoundException();

        _users.Remove(repoUser);
        
        _users.Add(user);
        
        return Task.CompletedTask;
    }

    public Task Add(User user)
    {
        _users.Add(user);
        
        return Task.CompletedTask;
    }
}