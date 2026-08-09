using Microsoft.AspNetCore.SignalR;

namespace Dara.Server.Apps.API;

public static class SignalRContextExtensions
{
    public static Guid GuidIdentifier(this HubCallerContext context)
    {
        if(!Guid.TryParse(context.UserIdentifier, out var guid))
            throw new HubException("User could not be identified withing domain scope");

        return guid;
    }
}