using Dara.Shared.Contracts.Abstractions;

namespace Dara.Shared.Contracts.Common;

public class FailureResponse : IAppResponse
{
    public string ErrorMessage { get; }
    
    public FailureResponse(string errorMessage)
    {
        ErrorMessage = errorMessage;
    }
}