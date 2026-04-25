using Dara.Shared.Contracts.Abstractions;

namespace Dara.Shared.Contracts.Common;

public class AppResponse : IAppResponse
{
    public AppResponseStatus Status { get; set; } = AppResponseStatus.Pending;
    
    public string ResponseMessage { get; set; } = string.Empty;

    public AppResponse()
    {
        
    }
    
    public void SetExpectedResponse(string responseMessage)
    {
        if(Status != AppResponseStatus.Pending)
            throw new ArgumentException("Cannot set expected response to a non-pending status");
        
        Status = AppResponseStatus.Success;
        ResponseMessage = responseMessage;
    }

    public void SetException(string exceptionMessage)
    {
        if(Status != AppResponseStatus.Pending)
            throw new ArgumentException("Cannot set exception to a non-pending status");
        
        Status = AppResponseStatus.Failure;
        ResponseMessage = exceptionMessage;
    }
}