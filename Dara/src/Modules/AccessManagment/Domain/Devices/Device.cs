using Dara.BuildingBlocks.Domain.Business;
using Dara.Modules.AccessManagment.Domain.Clients;

namespace Dara.Modules.AccessManagment.Domain.Devices;

public class Device : Entity, IAggregateRoot
{
    public DeviceName Name { get; private set; }
    
    public DeviceToken Token { get; private set; }

    public IReadOnlyCollection<Client> Clients => _clients.AsReadOnly();

    private readonly List<Client> _clients = new();

    public Device(DeviceName name, DeviceToken token)
    {
        Id = Guid.NewGuid();
        Name = name;
        Token = token;
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
    
    public void ChangeName(DeviceName newName)
    {
        Name = newName;
    }
    
    public void ChangeToken(DeviceToken newToken)
    {
        Token = newToken;
    }
}