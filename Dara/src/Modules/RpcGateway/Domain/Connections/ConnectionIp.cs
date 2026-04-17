using Dara.BuildingBlocks.Domain;

namespace Dara.Modules.RpcGateway.Domain.Connections;

public class ConnectionIp : ValueObject
{
    public string Value { get; }
    
    public ConnectionIp(string connectionIp)
    {
        Value = connectionIp;
    }
}