using Dara.BuildingBlocks.Domain.Business;
using Dara.Modules.AccessManagment.Domain.Clients;
using Dara.Modules.AccessManagment.Domain.Users.Events;

namespace Dara.Modules.AccessManagment.Domain.Users;

public class User : Entity, IAggregateRoot
{
    public IReadOnlyCollection<Client> Clients => _clients.AsReadOnly();

    private readonly List<Client> _clients = new();
    
    public UserEmail Email { get; private set; }
    
    public UserNickname Nickname { get; private set; }
    
    public UserPassword Password { get; private set; }

    public User(UserEmail email, UserNickname nickname, UserPassword password)
    {
        Id = Guid.NewGuid();
        
        Email = email;
        Nickname = nickname;
        Password = password;
        
        _events.Add(new UserCreatedEvent(this));
    }
    
    public void ChangeEmail(UserEmail email)
    {
        Email = email;
    }

    public void ChangeNickname(UserNickname nickname)
    {
        Nickname = nickname;
    }

    public void ChangePassword(UserPassword password)
    {
        Password = password;
    }
    
    public void AddClient(Client client)
    {
        _clients.Add(client);
        //brodcast event
    }

    public void RemoveClient(Client client)
    {
        _clients.Remove(client);
        //brodcast event
    }
}