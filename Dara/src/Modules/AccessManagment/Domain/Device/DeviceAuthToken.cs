using Dara.Core.Domain.Business;

namespace Dara.Modules.AccessManagment.Domain.Device;

public class DeviceAuthToken : ValueObject
{
    internal string Token { get; }
    
    internal DeviceAuthToken(string token)
    {
        Token = token;
    }
}