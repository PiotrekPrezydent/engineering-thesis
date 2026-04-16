using Dara.BuildingBlocks.Domain.Events;
using Dara.Modules.AccessManagment.Domain.Users;

namespace Dara.Modules.AccessManagment.Infrastructure;

public class UserRepository : IUserRepository
{
    private readonly List<User> _users;
    private readonly IDomainEventDispatcher _domainEventDispatcher;

    public UserRepository(IDomainEventDispatcher domainEventDispatcher)
    {
        _users = new();
        _domainEventDispatcher = domainEventDispatcher;
    }
    
    public async Task<User> FindByEmail(UserEmail email)
    {
        return _users.First(e => e.Email == email);
    }

    public async Task<User> FindByNickname(UserNickname nickname)
    {
        return _users.First(e => e.Nickname == nickname);
    }

    public async Task<User> FindById(Guid id)
    {
        return _users.First(e => e.Id == id);
    }

    public async Task Add(User user)
    {
        _users.Add(user);
        foreach (var domainEvent in user.DomainEvents)
        {
            await _domainEventDispatcher.DispatchAsync(domainEvent);
        }
        user.ClearDomainEvents();
    }

    public async Task Save(User user)
    {
        var userToRemove = _users.First(e => e.Id == user.Id);
        _users.Remove(userToRemove);
        _users.Add(user);
        
        foreach (var domainEvent in user.DomainEvents)
        {
            await _domainEventDispatcher.DispatchAsync(domainEvent);
        }
        user.ClearDomainEvents();
    }
}