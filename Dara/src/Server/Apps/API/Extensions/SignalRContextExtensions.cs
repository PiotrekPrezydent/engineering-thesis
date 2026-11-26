using Dara.Shared.Contracts.Auth;
using Microsoft.AspNetCore.SignalR;

namespace Dara.Server.Apps.API.Extensions;

public static class SignalRContextExtensions
{
    // the user id ("sub" of the token, through SubjectUserIdProvider)
    public static Guid GuidIdentifier(this HubCallerContext context)
    {
        if(!Guid.TryParse(context.UserIdentifier, out var guid))
            throw new HubException("User could not be identified withing domain scope");

        return guid;
    }

    // the hub takes device tokens only, so the claim is always there
    public static Guid DeviceId(this HubCallerContext context)
    {
        if (!Guid.TryParse(context.User?.FindFirst(AuthClaims.Device)?.Value, out var guid))
            throw new HubException("Device could not be identified");

        return guid;
    }
}
