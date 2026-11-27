using System.Security.Claims;
using Dara.Shared.Contracts.Auth;

namespace Dara.Server.Apps.API.Common.Authentication;

// claims of a validated client token or of the panel cookie; MapInboundClaims is off, so the claims keep their jwt
// names; commands of the modules get these ids, never a session or a token
public static class ClaimsPrincipalExtensions
{
    public static Guid UserId(this ClaimsPrincipal principal)
    {
        return RequiredGuid(principal, AuthClaims.Subject);
    }

    public static Guid ClientId(this ClaimsPrincipal principal)
    {
        return RequiredGuid(principal, AuthClaims.Client);
    }

    public static Guid PanelSignInId(this ClaimsPrincipal principal)
    {
        return RequiredGuid(principal, AuthClaims.Session);
    }

    // the subject Authorization checks: the client of a client token, the panel client of the account of the panel
    // cookie; each has only its own roles (a client does not get the roles of the panel), the account itself has none
    public static Guid SenderId(this ClaimsPrincipal principal)
    {
        return principal.ClientId();
    }

    public static bool IsClient(this ClaimsPrincipal principal)
    {
        return principal.HasClaim(AuthClaims.Scope, AuthClaims.ScopeClient) && principal.HasClaim(c => c.Type == AuthClaims.Client);
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
