using Microsoft.AspNetCore.SignalR;

namespace Dara.Server.Apps.API;

public static class SignalRContextExtensions
{
    private const string UserIdKey = "DomainUserId";

    public static Guid GetDomainUserId(this HubCallerContext context)
    {
        if (context.Items.TryGetValue(UserIdKey, out var userIdObj) && userIdObj is Guid userId)
        {
            return userId;
        }
        
        throw new HubException("User could not be identified withing domain scope");
    }
}