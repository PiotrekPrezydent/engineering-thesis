using Dara.BuildingBlocks.Domain;

namespace Dara.Modules.RpcGateway.Domain.Clients;

public class ClientName : ValueObject
{
    public string Name { get; }

    public ClientName(string name)
    {
        if(!IsNameValid(name))
            throw new ArgumentException($"{name} is not a valid client name.", nameof(name));
        
        Name = name;
    }

    //implement
    bool IsNameValid(string name)
    {
        return true;
    }
}