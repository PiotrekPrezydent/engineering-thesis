using Dara.Shared.Contracts.Clients;
using Dara.Shared.Contracts.Common;

namespace Dara.Shared.Contracts.Interactions
{
    //define every use case fore connections
    public interface IConnectionInteractions
    {
        public Task<AppResponse> ActiveClientAsync(ClientActivationDto client);
    
        public Task<AppResponse> DeactiveClientAsync();
    }
}