using Dara.BuildingBlocks.Domain;

namespace Dara.Modules.Communication.Domain.Clients;

public class ClientAuthToken : ValueObject
{
    public string Token { get; }
    
    public ClientAuthToken(string token)
    {
        Token = token;
    }
}