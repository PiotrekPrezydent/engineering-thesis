using System.Security.Claims;
using Dara.Shared.Contracts.Auth;

namespace Dara.Server.Apps.API.Authentication;

// claims of a validated access token; MapInboundClaims is off, so the claims keep their jwt names
public static class ClaimsPrincipalExtensions
{
    public static Guid UserId(this ClaimsPrincipal principal)
    {
        return RequiredGuid(principal, AuthClaims.Subject);
    }

    public static Guid DeviceId(this ClaimsPrincipal principal)
    {
        return RequiredGuid(principal, AuthClaims.Device);
    }

    public static Guid AccountSessionId(this ClaimsPrincipal principal)
    {
        return RequiredGuid(principal, AuthClaims.Session);
    }

    public static bool IsDevice(this ClaimsPrincipal principal)
    {
        return principal.HasClaim(AuthClaims.Scope, AuthClaims.ScopeDevice) && principal.HasClaim(c => c.Type == AuthClaims.Device);
    }

    public static bool IsAccount(this ClaimsPrincipal principal)
    {
        return principal.HasClaim(AuthClaims.Scope, AuthClaims.ScopeAccount) && principal.HasClaim(c => c.Type == AuthClaims.Session);
    }

    private static Guid RequiredGuid(ClaimsPrincipal principal, string claimType)
    {
        var value = principal.FindFirst(claimType)?.Value;
        if (!Guid.TryParse(value, out var guid))
            throw new InvalidOperationException($"the access token has no valid '{claimType}' claim");

        return guid;
    }
}
