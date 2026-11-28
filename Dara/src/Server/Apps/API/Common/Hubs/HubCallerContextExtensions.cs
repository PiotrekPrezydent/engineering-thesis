using Dara.Shared.Contracts.Auth;
using Microsoft.AspNetCore.SignalR;

namespace Dara.Server.Apps.API.Common.Hubs;

public static class HubCallerContextExtensions
{
    // the hub takes client tokens only, so the claim is always there; it is the id of the member, the plugin owner
    // and the participant
    public static Guid ClientId(this HubCallerContext context)
    {
        if (!Guid.TryParse(context.User?.FindFirst(AuthClaims.Client)?.Value, out var guid))
            throw new HubException("Client could not be identified");

        return guid;
    }

    // the account of the client ("sub" of the token)
    public static Guid UserId(this HubCallerContext context)
    {
        if (!Guid.TryParse(context.User?.FindFirst(AuthClaims.Subject)?.Value, out var guid))
            throw new HubException("User could not be identified");

        return guid;
    }
}
