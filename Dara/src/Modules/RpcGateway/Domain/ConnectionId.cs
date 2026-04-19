using Dara.BuildingBlocks.Domain;
using Dara.BuildingBlocks.Domain.Exceptions;

namespace Dara.Modules.RpcGateway.Domain;

public class ConnectionId : ValueObject
{
    public string Value { get; }

    public ConnectionId(string connectionId)
    {
        if(!IsValid(connectionId))
            throw new ValueObjectIsNotValidException<ConnectionId>(this, nameof(connectionId), connectionId, "ConnectionId is invalid");
        
        Value = connectionId;
    }

    bool IsValid(string connectionId)
    {
        return true;
    }
}