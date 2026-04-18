using Dara.BuildingBlocks.Domain;

namespace Dara.Modules.RpcGateway.Domain;

public class ConnectionIp : ValueObject
{
    public string Value { get; }
    
    public ConnectionIp(string connectionIp)
    {
        Value = connectionIp;
    }
}