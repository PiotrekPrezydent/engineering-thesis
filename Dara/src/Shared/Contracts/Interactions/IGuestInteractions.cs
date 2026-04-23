using Dara.Shared.Contracts.Common;

namespace Dara.Shared.Contracts.Interactions;

//define every use case fore connections
public interface IGuestInteractions
{
    public Task<AppResponse> EnableClientNodeAsync();
    
    public Task<AppResponse> DisableClientNodeAsync();
}