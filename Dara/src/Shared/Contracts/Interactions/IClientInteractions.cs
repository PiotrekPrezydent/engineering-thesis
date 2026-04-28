using Dara.Shared.Common;
using Dara.Shared.Contracts.Dtos;

namespace Dara.Shared.Contracts.Interactions
{
    public interface IActiveConnectionInteractions
    {
        public Task<WrappedResult<MessageDto>> ChangeClientNameAsync(ClientNameDto clientName);

        public Task<WrappedResult<MessageDto>> ChangeClientAuthTokenAsync(ClientAuthTokenDto clientAuthToken);
    }
}