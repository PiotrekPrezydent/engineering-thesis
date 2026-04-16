using Dara.BuildingBlocks.Domain;

namespace Dara.Modules.RpcGateway.Domain;

public class ClientName : ValueObject
{
    public string Name { get; }

    public ClientName(string name)
    {
        Name = name;
    }
}