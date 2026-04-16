using Dara.BuildingBlocks.Domain.Business;

namespace Dara.Modules.RpcGateway.Domain.Connection;

public class ConnectionId : ValueObject
{
    public string Value { get; }

    public ConnectionId(string connectionId)
    {
        Value = connectionId;
    }
}