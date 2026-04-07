using Dara.Core.Domain.Business;
using Dara.Modules.AccessManagment.Domain.Device;
using Dara.Modules.AccessManagment.Domain.Events;
using Dara.Modules.AccessManagment.Domain.Exceptions;

namespace Dara.Modules.AccessManagment.Domain.User;

public class User : Entity, IAggregateRoot
{
    public IReadOnlyCollection<DeviceIdentity> Devices => _devices.AsReadOnly();
    private readonly List<DeviceIdentity> _devices = new();
    
    public UserEmail Email { get; private set; }
    public UserPassword Password { get; private set; }
    public UserNickname Nickname { get; private set; }

    //for user registration
    public User(string email, string hashedPassword, string nickname)
    {
        Id = Guid.NewGuid();
        Email = new UserEmail(email);
        Password = new UserPassword(hashedPassword);
        Nickname = new UserNickname(nickname);
        
        _events.Add(new UserCreatedEvent(Id));
    }

    public void ChangeEmail(string newEmail)
    {
        Email = new UserEmail(newEmail);
    }

    public void ChangePassword(string hashedPassword)
    {
        Password = new UserPassword(hashedPassword);
    }

    public void ChangeNickname(string nickname)
    {
        Nickname = new UserNickname(nickname);
    }
    
    public void AddDevice(string deviceName, string deviceToken)
    {
        DeviceIdentity device = new(Id, deviceName, new(deviceToken));
        _devices.Add(device);
        
        _events.Add(new UserDeviceAddedEvent(Id,  device.Id));
    }

    public void RemoveDevice(string deviceName)
    {
        var device = _devices.Find(e => e.DeviceName == deviceName);
        if (device == null)
            throw new DeviceNotFoundException();
        
        _devices.Remove(device);
        _events.Add(new UserDeviceRemovedEvent(Id, device.Id));
    }
    
}