using Dara.Shared.Common;
using Dara.Shared.Contracts.Dtos;

namespace Dara.Shared.Contracts.Interactions
{
    //define every use case fore connections
    public interface IConnectionInteractions
    {
        public Task<WrappedResult<MessageDto>> ActiveClientAsync(ClientActivationDto client);
    
        public Task<WrappedResult<MessageDto>> DeactiveClientAsync();
    }
}