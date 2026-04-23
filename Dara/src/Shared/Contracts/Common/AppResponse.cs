using Dara.Shared.Contracts.Abstractions;

namespace Dara.Shared.Contracts.Common;

public class AppResponse : IAppResponse
{
    public AppResponseType ResponseType;

    public IAppResponse Response;

    public AppResponse(AppResponseType responseType, IAppResponse response)
    {
        ResponseType = responseType;
        Response = response;
    }

    public T ProvideResponse<T>() where T : IAppResponse
    {
        if (ResponseType != AppResponseType.Success)
            return null; // exception
        
        return (T)Response;
    }
}