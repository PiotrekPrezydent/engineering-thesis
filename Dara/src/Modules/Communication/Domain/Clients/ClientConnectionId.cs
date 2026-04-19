using Dara.BuildingBlocks.Domain;
using Dara.BuildingBlocks.Domain.Exceptions;

namespace Dara.Modules.Communication.Domain.Clients;

public class ClientConnectionId : ValueObject
{
    public string Value { get; }

    public ClientConnectionId(string connectionId)
    {
        if(!IsValid(connectionId))
            throw new ValueObjectIsNotValidException<ClientConnectionId>(this, nameof(connectionId), connectionId, "ConnectionId is invalid");
        
        Value = connectionId;
    }

    bool IsValid(string connectionId)
    {
        return true;
    }
}