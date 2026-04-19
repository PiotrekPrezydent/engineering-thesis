using Dara.BuildingBlocks.Domain;
using Dara.BuildingBlocks.Domain.Exceptions;

namespace Dara.Modules.RpcGateway.Domain;

public class ConnectionIp : ValueObject
{
    public string Value { get; }
    
    public ConnectionIp(string connectionIp)
    {
        if(!IsValid(connectionIp))
            throw new ValueObjectIsNotValidException<ConnectionIp>(this, nameof(connectionIp), connectionIp, "ConnectionIp is invalid");
        
        Value = connectionIp;
    }

    bool IsValid(string connectionIp)
    {
        return true;
    }
}