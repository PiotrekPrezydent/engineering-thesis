using Microsoft.AspNetCore.SignalR;

namespace Dara.Server.Apps.API;

public class HeaderUserIdProvider : IUserIdProvider
{
    public string GetUserId(HubConnectionContext connection)
    {
        var httpContext = connection.GetHttpContext();
        
        var clientId = httpContext?.Request.Headers["X-Client-Id"].FirstOrDefault();
        
        return clientId;
    }
}