using Dara.BuildingBlocks.Domain;

namespace Dara.Modules.RpcGateway.Domain.Connections;

public class ConnectionId : ValueObject
{
    public string Value { get; }

    public ConnectionId(string connectionId)
    {
        Value = connectionId;
    }
}