using Dara.BuildingBlocks.Domain;

namespace Dara.Modules.Communication.Domain.Clients;

public class ClientName : ValueObject
{
    public string Value { get; }
    
    public ClientName(string value)
    {
        Value = value;
    }
}