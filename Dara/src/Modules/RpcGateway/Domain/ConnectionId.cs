using Dara.BuildingBlocks.Domain.Business;

namespace Dara.Modules.RpcGateway.Domain;

public class ConnectionId : ValueObject
{
    public string Value { get; }

    public ConnectionId(string connectionId)
    {
        Value = connectionId;
    }
}