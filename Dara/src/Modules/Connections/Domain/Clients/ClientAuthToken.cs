using Dara.BuildingBlocks.Domain.Models;
using Dara.Modules.Connections.Domain.Clients.Rules;

namespace Dara.Modules.Connections.Domain.Clients
{
    public class ClientAuthToken : ValueObject
    {
        public string Value { get; }
    
        public ClientAuthToken(string value)
        {
            CheckRule(new ClientAuthTokenCannotBeEmptyRule(value));
        
            Value = value;
        }
    }
}