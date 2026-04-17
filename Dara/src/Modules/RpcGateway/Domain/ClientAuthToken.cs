using Dara.BuildingBlocks.Domain;

namespace Dara.Modules.RpcGateway.Domain;

public class ClientAuthToken : ValueObject
{
    public string AuthToken  { get; }

    public ClientAuthToken(string authToken)
    {
        AuthToken = authToken;
    }
}