using Dara.Shared.Contracts.Abstractions;

namespace Dara.Shared.Contracts.Common;

public class AppResponse : IAppResponse
{
    public AppResponseStatus Status { get; set; } = AppResponseStatus.Pending;

    //public IAppResponse? ExpectedResponse { get;set; }
    
    public string StatusS { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
    
    //public Exception? ResponseException { get; private set; }
    
    //public DateTime CreatedAt { get; private set; }
    //public DateTime UpdatedAt { get; private set; }

    public AppResponse()
    {
        
    }

    // public AppResponse()
    // {
    //     Console.WriteLine("INIT APP RESPONSE");
    //     Status = AppResponseStatus.Pending;
    //     
    //     ExpectedResponse = null;
    //     //ResponseException = null;
    //     
    //     // CreatedAt = DateTime.Now;
    //     // UpdatedAt = DateTime.Now;
    // }

    public void SetExpectedResponse(IAppResponse expectedResponse)
    {
        if(Status != AppResponseStatus.Pending)
            throw new ArgumentException("Cannot set expected response to a non-pending status");
        
        // if(ResponseException != null)
        //     throw new ArgumentException("Cannot set response after exception is already set", nameof(ResponseException));
        
        // if(ExpectedResponse != null)
        //     throw new ArgumentException("ExpectedResponse is already set", nameof(expectedResponse));
        //
        // Status = AppResponseStatus.Success;
        // ExpectedResponse = expectedResponse;
        
        // UpdatedAt = DateTime.UtcNow;
    }

    public void SetException()
    {
        if(Status != AppResponseStatus.Pending)
            throw new ArgumentException("Cannot set exception to a non-pending status");
        
        // if(ExpectedResponse != null)
        //     throw new ArgumentException("Cannot set exception after response is already set", nameof(ResponseException));
        
        // if(ResponseException != null)
        //     throw new ArgumentException("ExpectedResponse is already set", nameof(exception));
        
        Status = AppResponseStatus.Failure;
        //ResponseException = exception;
        
        // UpdatedAt = DateTime.UtcNow;
    }

    public T GetResponse<T>() where T : IAppResponse
    {
        if (Status != AppResponseStatus.Success)
            throw new ArgumentException("Cannot take response after failing");
        
        // if(ExpectedResponse == null)
        //     throw new NullReferenceException("ExpectedResponse is null");
        //
        // return (T)ExpectedResponse;
        return default;
    }
}