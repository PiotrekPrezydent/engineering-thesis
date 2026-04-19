using Dara.BuildingBlocks.Domain;
using Dara.BuildingBlocks.Domain.Exceptions;

namespace Dara.Modules.Communication.Domain.Clients;

public class ClientConnectionIp : ValueObject
{
    public string Value { get; }
    
    public ClientConnectionIp(string connectionIp)
    {
        if(!IsValid(connectionIp))
            throw new ValueObjectIsNotValidException<ClientConnectionIp>(this, nameof(connectionIp), connectionIp, "ConnectionIp is invalid");
        
        Value = connectionIp;
    }

    bool IsValid(string connectionIp)
    {
        return true;
    }
}