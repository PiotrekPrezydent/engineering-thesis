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
    
    public void AddDevice(string deviceName, DeviceAuthToken deviceToken)
    {
        DeviceIdentity device = new(Id, deviceName, deviceToken);
        _devices.Add(device);
        
        _events.Add(new UserDeviceAddedEvent(Id,  device.Id));
    }

    public void RemoveDevice(Guid deviceId)
    {
        var device = _devices.Find(e => e.Id == deviceId);
        if (device == null)
            throw new DeviceNotFoundException();
        
        _devices.Remove(device);
        _events.Add(new UserDeviceRemovedEvent(Id, device.Id));
    }
    
}