using System.Security.Claims;
using Dara.Server.Modules.Identity.Application;
using Dara.Server.Modules.Identity.Application.AuthenticatePanelSignIn;
using Dara.Shared.Contracts.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Dara.Server.Apps.API.Common.Authentication;

// the session of the account panel: the cookie holds the id of a sign-in of the panel (PanelSignIn of the identity
// module), which the server checks on every request, so a sign-out ends it at once, and the id of the panel client of
// the account, its subject in Authorization; the panel is served by this host (same site)
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

    public static ClaimsPrincipal CreatePrincipal(Guid userId, Guid panelClientId, Guid panelSignInId)
    {
        var identity = new ClaimsIdentity(
        [
            new Claim(AuthClaims.Subject, userId.ToString()),
            new Claim(AuthClaims.Client, panelClientId.ToString()),
            new Claim(AuthClaims.Session, panelSignInId.ToString()),
            new Claim(AuthClaims.Scope, AuthClaims.ScopeAccount)
        ], Scheme, AuthClaims.Subject, null);

        return new ClaimsPrincipal(identity);
    }

    // an ended, idle or unknown sign-in (or one of another user or panel client) drops the cookie
    private static async Task ValidateAsync(CookieValidatePrincipalContext context)
    {
        var principal = context.Principal;
        var userId = principal?.FindFirst(AuthClaims.Subject)?.Value;
        var panelClientId = principal?.FindFirst(AuthClaims.Client)?.Value;
        PanelSignInResult? valid = null;

        if (Guid.TryParse(principal?.FindFirst(AuthClaims.Session)?.Value, out var signInId))
        {
            var identity = context.HttpContext.RequestServices.GetRequiredService<IIdentityModule>();
            valid = await identity.AuthenticatePanelSignInAsync(signInId);
        }

        if (valid == null || valid.UserId.ToString() != userId || valid.PanelClientId.ToString() != panelClientId)
        {
            context.RejectPrincipal();
            await context.HttpContext.SignOutAsync(Scheme);
        }
    }
}
