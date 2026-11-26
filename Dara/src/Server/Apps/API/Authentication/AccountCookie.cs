using System.Security.Claims;
using Dara.Server.Modules.Identity.Application;
using Dara.Server.Modules.Identity.Application.ValidateAccountSession;
using Dara.Shared.Contracts.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Dara.Server.Apps.API.Authentication;

// the session of the account panel: the cookie holds the id of an AccountSession of the identity module, which the
// server checks on every request, so a sign-out ends it at once; the panel is served by this host (same site)
public static class AccountCookie
{
    public const string Scheme = "AccountCookie";
    public const string CookieName = "dara.account";

    // the cookie may live as long as the session; the session itself ends after 30 days without use
    public static readonly TimeSpan Lifetime = TimeSpan.FromDays(30);

    public static void Configure(CookieAuthenticationOptions options)
    {
        options.Cookie.Name = CookieName;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.ExpireTimeSpan = Lifetime;
        options.SlidingExpiration = true;

        options.Events = new CookieAuthenticationEvents
        {
            OnValidatePrincipal = ValidateAsync,
            // an api answers with a status code, there is no login page to redirect to
            OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            },
            OnRedirectToAccessDenied = context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return Task.CompletedTask;
            }
        };
    }

    public static ClaimsPrincipal CreatePrincipal(Guid userId, Guid accountSessionId)
    {
        var identity = new ClaimsIdentity(
        [
            new Claim(AuthClaims.Subject, userId.ToString()),
            new Claim(AuthClaims.Session, accountSessionId.ToString()),
            new Claim(AuthClaims.Scope, AuthClaims.ScopeAccount)
        ], Scheme, AuthClaims.Subject, null);

        return new ClaimsPrincipal(identity);
    }

    // an ended, idle or unknown session (or one of another user) drops the cookie
    private static async Task ValidateAsync(CookieValidatePrincipalContext context)
    {
        var principal = context.Principal;
        var userId = principal?.FindFirst(AuthClaims.Subject)?.Value;
        Guid? validUserId = null;

        if (Guid.TryParse(principal?.FindFirst(AuthClaims.Session)?.Value, out var sessionId))
        {
            var identity = context.HttpContext.RequestServices.GetRequiredService<IIdentityModule>();
            validUserId = await identity.ExecuteCommandAsync<ValidateAccountSessionCommand, Guid?>(new ValidateAccountSessionCommand(sessionId));
        }

        if (validUserId == null || validUserId.Value.ToString() != userId)
        {
            context.RejectPrincipal();
            await context.HttpContext.SignOutAsync(Scheme);
        }
    }
}
