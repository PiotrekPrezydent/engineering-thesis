using Dara.BuildingBlocks.Domain.Models;
using Dara.Modules.Communication.Domain.Clients.Rules;

namespace Dara.Modules.Communication.Domain.Clients;

public class ClientName : ValueObject
{
    public string Value { get; }
    
    public ClientName(string value)
    {
        CheckRule(new ClientNameCannotBeEmptyRule(value));
        
        Value = value;
    }
}