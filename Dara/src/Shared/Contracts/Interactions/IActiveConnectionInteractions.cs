using Dara.Shared.Contracts.Clients;
using Dara.Shared.Contracts.Common;

namespace Dara.Shared.Contracts.Interactions;

//define every use case for Node
public interface IActiveConnectionInteractions
{
    public Task<AppResponse> ChangeClientNameAsync(ClientNameDto clientName);

    public Task<AppResponse> ChangeClientAuthTokenAsync(ClientAuthTokenDto clientAuthToken);
}