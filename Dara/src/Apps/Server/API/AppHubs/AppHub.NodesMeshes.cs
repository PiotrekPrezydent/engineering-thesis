using Dara.Shared.Contracts.Common;
using Dara.Shared.Contracts.Interactions;

namespace Dara.Apps.Server.API.AppHubs;

public partial class AppHub : INodeMeshInteractions
{
    public async Task<AppResponse> ChangeMeshNameAsync()
    {
        // var command = new ChangeNodesMeshNameCommand();
        // AppResponse response;
        //
        // try
        // {
        //     var result = await _applicationCommandDispatcher.DispatchAsync<ChangeNodesMeshNameCommand,ChangeNodesMeshNameCommandResult>(command);
        //     
        //     var buildedResponse = new IAppResponse();
        //     
        //     response = new(AppResponseType.Success, buildedResponse);
        // }
        // catch (Exception ex)
        // {
        //     if (ex.GetType().IsAssignableFrom(typeof(BaseDomainException)))
        //         (ex as BaseDomainException).PrintBuildedMessage();
        //     
        //     response = new(AppResponseType.Failure, new FailureResponse(ex.Message));
        // }
        //
        // return response;
        return null;
    }

    public Task<AppResponse> ChangeMeshCodeAsync()
    {
        throw new NotImplementedException();
    }
}