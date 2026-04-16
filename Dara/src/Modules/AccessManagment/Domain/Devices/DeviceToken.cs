using Dara.BuildingBlocks.Domain.Business;

namespace Dara.Modules.AccessManagment.Domain.Devices;

public class DeviceToken : ValueObject
{
    public string Token { get; }
    
    public DeviceToken(string token)
    {
        Token = token;
    }
}