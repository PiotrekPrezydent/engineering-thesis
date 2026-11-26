using System.Security.Claims;
using Dara.Server.Apps.API.Authentication;
using Dara.Server.Modules.Identity.Application;
using Dara.Server.Modules.Identity.Application.IssueDeviceLoginCode;
using Dara.Server.Modules.Identity.Application.RefreshSession;
using Dara.Server.Modules.Identity.Application.SignIn;
using Dara.Server.Modules.Identity.Application.SignOut;
using Dara.Shared.Contracts.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Apps.API.Endpoints;

// sign-in over http: the account panel with login and password (a session cookie), a device with a one-time code
// from the panel (device tokens); the identity module keeps accounts and sessions, the host issues the tokens
// and the cookie; accounts come from the seed for now, there is no registration endpoint
public static class AuthEndpoints
{
    // the rate limiter policy of device-login: a code has about 40 bits, a limit per address keeps guessing slow
    public const string DeviceLoginRateLimit = "device-login";

    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(AuthRoutes.Group);

        group.MapPost("/login", LoginAsync).AllowAnonymous();
        group.MapPost("/device-login", DeviceLoginAsync).AllowAnonymous().RequireRateLimiting(DeviceLoginRateLimit);
        group.MapPost("/refresh", RefreshAsync).AllowAnonymous();
        // a device token or the panel cookie: the claims say which session ends
        group.MapPost("/logout", LogoutAsync).RequireAuthorization();

        // other endpoints of the panel under /account/devices come from the devices module (TASK-ENG-029)
        var accountDevices = app.MapGroup(AuthRoutes.AccountDevicesGroup).RequireAuthorization(AuthPolicies.Account);
        accountDevices.MapPost("/{deviceId:guid}/login-code", IssueDeviceLoginCodeAsync);

        return app;
    }

    private static async Task<IResult> LoginAsync(LoginRequest request, HttpContext http, [FromServices] IIdentityModule identity)
    {
        var result = await identity.ExecuteCommandAsync<SignInAccountCommand, AccountSignInResult>(
            new SignInAccountCommand(request.Login, request.Password));

        if (!result.Succeeded)
            return Refused(result.Error!.Value);

        await http.SignInAsync(AccountCookie.Scheme, AccountCookie.CreatePrincipal(result.UserId, result.AccountSessionId),
            new AuthenticationProperties { IsPersistent = true });

        return TypedResults.Ok(new AccountSignInResponse(result.UserId));
    }

    private static async Task<IResult> DeviceLoginAsync(DeviceLoginRequest request, [FromServices] IIdentityModule identity, [FromServices] AccessTokenIssuer issuer)
    {
        DeviceTokensResult result;
        try
        {
            result = await identity.ExecuteCommandAsync<SignInDeviceCommand, DeviceTokensResult>(new SignInDeviceCommand(request.Code));
        }
        catch (DbUpdateConcurrencyException)
        {
            // another sign-in with the same code was saved first
            return Refused(AuthError.InvalidLoginCode);
        }

        return result.Succeeded ? TypedResults.Ok(Tokens(issuer, result)) : Refused(result.Error!.Value);
    }

    private static async Task<IResult> RefreshAsync(RefreshRequest request, [FromServices] IIdentityModule identity, [FromServices] AccessTokenIssuer issuer)
    {
        DeviceTokensResult result;
        try
        {
            result = await identity.ExecuteCommandAsync<RefreshSessionCommand, DeviceTokensResult>(new RefreshSessionCommand(request.RefreshToken));
        }
        catch (DbUpdateConcurrencyException)
        {
            // another refresh with the same token was saved first; this one is refused without ending the session
            return Refused(AuthError.RefreshInvalid);
        }

        return result.Succeeded ? TypedResults.Ok(Tokens(issuer, result)) : Refused(result.Error!.Value);
    }

    // a device: its refresh tokens end, the access token stays valid until it expires (at most 15 minutes);
    // the panel: its session ends at once and the cookie is removed
    private static async Task<IResult> LogoutAsync(ClaimsPrincipal user, HttpContext http, [FromServices] IIdentityModule identity)
    {
        if (user.IsDevice())
            await identity.ExecuteCommandAsync(new SignOutDeviceCommand(user.UserId(), user.DeviceId()));

        if (user.IsAccount())
        {
            await identity.ExecuteCommandAsync(new SignOutAccountCommand(user.UserId(), user.AccountSessionId()));
            await http.SignOutAsync(AccountCookie.Scheme);
        }

        return TypedResults.NoContent();
    }

    private static async Task<IResult> IssueDeviceLoginCodeAsync(Guid deviceId, ClaimsPrincipal user, [FromServices] IIdentityModule identity)
    {
        var result = await identity.ExecuteCommandAsync<IssueDeviceLoginCodeCommand, DeviceLoginCodeResult>(
            new IssueDeviceLoginCodeCommand(user.UserId(), deviceId));

        return result.Succeeded
            ? TypedResults.Ok(new DeviceLoginCodeResponse(result.Code!, result.ExpiresAt!.Value))
            : Refused(result.Error!.Value);
    }

    private static AuthTokensResponse Tokens(AccessTokenIssuer issuer, DeviceTokensResult result)
    {
        var access = issuer.IssueDevice(result.UserId, result.DeviceId);
        return new AuthTokensResponse(access.Token, access.ExpiresAt, result.RefreshToken!, result.RefreshTokenExpiresAt!.Value,
            result.UserId, result.DeviceId);
    }

    private static IResult Refused(AuthError error)
    {
        return error switch
        {
            AuthError.InvalidCredentials => Problem(StatusCodes.Status401Unauthorized, AuthErrorCodes.InvalidCredentials, "Wrong login or password"),
            AuthError.InvalidLoginCode => Problem(StatusCodes.Status401Unauthorized, AuthErrorCodes.InvalidLoginCode, "The code is not valid, get a new one in the account panel"),
            AuthError.DeviceNotFound => Problem(StatusCodes.Status404NotFound, AuthErrorCodes.DeviceNotFound, "No such device of this account"),
            AuthError.DeviceRevoked => Problem(StatusCodes.Status409Conflict, AuthErrorCodes.DeviceRevoked, "The device was revoked"),
            AuthError.RefreshExpired => Problem(StatusCodes.Status401Unauthorized, AuthErrorCodes.RefreshExpired, "The refresh token expired, sign in with a new code"),
            AuthError.RefreshReused => Problem(StatusCodes.Status401Unauthorized, AuthErrorCodes.RefreshReused, "The refresh token was used before, the session is ended, sign in with a new code"),
            AuthError.RefreshRevoked => Problem(StatusCodes.Status401Unauthorized, AuthErrorCodes.RefreshRevoked, "The session is ended, sign in with a new code"),
            _ => Problem(StatusCodes.Status401Unauthorized, AuthErrorCodes.RefreshInvalid, "The refresh token is not valid, sign in with a new code")
        };
    }

    private static IResult Problem(int status, string code, string detail)
    {
        return TypedResults.Problem(detail: detail, statusCode: status, extensions: new Dictionary<string, object?> { ["code"] = code });
    }
}
