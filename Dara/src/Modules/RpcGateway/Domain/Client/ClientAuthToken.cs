using Dara.BuildingBlocks.Domain.Business;

namespace Dara.Modules.RpcGateway.Domain.Client;

public class ClientAuthToken : ValueObject
{
    public string AuthToken  { get; }

    public ClientAuthToken(string authToken)
    {
        AuthToken = authToken;
    }
}