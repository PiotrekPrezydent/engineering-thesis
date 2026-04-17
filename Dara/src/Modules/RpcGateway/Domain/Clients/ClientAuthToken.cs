using Dara.BuildingBlocks.Domain;

namespace Dara.Modules.RpcGateway.Domain.Clients;

public class ClientAuthToken : ValueObject
{
    public string AuthToken  { get; }

    public ClientAuthToken(string authToken)
    {
        if(!IsTokenValid(authToken))
            throw new ArgumentException($"{authToken} is not valid client auth token",  nameof(authToken));
        
        AuthToken = authToken;
    }

    //implement
    bool IsTokenValid(string authToken)
    {
        return true;
    }
}