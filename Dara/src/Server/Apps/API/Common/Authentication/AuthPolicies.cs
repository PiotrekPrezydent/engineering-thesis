using Dara.Shared.Contracts.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Dara.Server.Apps.API.Common.Authentication;

// Client: the client token (jwt), for the hub and endpoints of a client; Account: the cookie of the account panel,
// for its endpoints; anything else without an explicit policy or AllowAnonymous needs either of them (the default
// and the fallback policy)
public static class AuthPolicies
{
    public const string Client = "Client";
    public const string Account = "Account";

    public static void Configure(AuthorizationOptions options)
    {
        var anySession = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme, AccountCookie.Scheme)
            .RequireAuthenticatedUser()
            .Build();
        options.DefaultPolicy = anySession;
        options.FallbackPolicy = anySession;

        options.AddPolicy(Client, policy => policy
            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser()
            .RequireClaim(AuthClaims.Scope, AuthClaims.ScopeClient)
            .RequireClaim(AuthClaims.Client));

        options.AddPolicy(Account, policy => policy
            .AddAuthenticationSchemes(AccountCookie.Scheme)
            .RequireAuthenticatedUser()
            .RequireClaim(AuthClaims.Scope, AuthClaims.ScopeAccount)
            .RequireClaim(AuthClaims.Session));
    }
}
