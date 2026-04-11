using Dara.Core.Domain.Business;
using Dara.Modules.AccessManagment.Domain.Events;
using Dara.Modules.AccessManagment.Domain.Exceptions;
using Dara.Modules.AccessManagment.Domain.Node;

namespace Dara.Modules.AccessManagment.Domain.User;

public class User : Entity, IAggregateRoot
{
    public IReadOnlyCollection<NodeIdentity> Nodes => _nodes.AsReadOnly();
    private readonly List<NodeIdentity> _nodes = new();
    
    public UserEmail Email { get; private set; }
    public UserPassword Password { get; private set; }
    public UserNickname Nickname { get; private set; }

    //for user registration
    public User(UserEmail email, UserPassword hashedPassword, UserNickname nickname)
    {
        Id = Guid.NewGuid();
        Email = email;
        Password = hashedPassword;
        Nickname = nickname;
        
        _events.Add(new UserCreatedEvent(Id));
    }

    public void ChangeEmail(UserEmail newEmail)
    {
        Email = newEmail;
    }

    public void ChangePassword(UserPassword newPassword)
    {
        Password = newPassword;
    }

    public void ChangeNickname(UserNickname newNickname)
    {
        Nickname = newNickname;
    }
    
    public void AddNode(string nodeName, NodeAuthToken nodeToken)
    {
        NodeIdentity node = new(Id, nodeName, nodeToken);
        _nodes.Add(node);
        
        _events.Add(new UserNodeAddedEvent(Id,  node.Id));
    }

    public void RemoveNode(Guid nodeId)
    {
        var device = _nodes.Find(e => e.Id == nodeId);
        if (device == null)
            throw new NodeNotFoundException();
        
        _nodes.Remove(device);
        _events.Add(new UserNodeRemovedEvent(Id, device.Id));
    }
    
}