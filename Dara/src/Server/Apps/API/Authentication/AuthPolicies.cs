using Dara.Shared.Contracts.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Dara.Server.Apps.API.Authentication;

// Device: the device token (jwt), for the hub and endpoints of a device; Account: the cookie of the account panel,
// for its endpoints; anything else without an explicit policy or AllowAnonymous needs either of them (the default
// and the fallback policy)
public static class AuthPolicies
{
    public const string Device = "Device";
    public const string Account = "Account";

    public static void Configure(AuthorizationOptions options)
    {
        var anySession = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme, AccountCookie.Scheme)
            .RequireAuthenticatedUser()
            .Build();
        options.DefaultPolicy = anySession;
        options.FallbackPolicy = anySession;

        options.AddPolicy(Device, policy => policy
            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser()
            .RequireClaim(AuthClaims.Scope, AuthClaims.ScopeDevice)
            .RequireClaim(AuthClaims.Device));

        options.AddPolicy(Account, policy => policy
            .AddAuthenticationSchemes(AccountCookie.Scheme)
            .RequireAuthenticatedUser()
            .RequireClaim(AuthClaims.Scope, AuthClaims.ScopeAccount)
            .RequireClaim(AuthClaims.Session));
    }
}
