using System.Security.Claims;
using Dara.Server.Apps.API.Common.Authentication;
using Dara.Server.Modules.Identity.Application;
using Dara.Server.Modules.Identity.Application.CreateClientRegistrationCode;
using Dara.Server.Modules.Identity.Application.IssueClientLoginCode;
using Dara.Server.Modules.Identity.Application.RefreshSession;
using Dara.Server.Modules.Identity.Application.RegisterClient;
using Dara.Server.Modules.Identity.Application.SignIn;
using Dara.Server.Modules.Identity.Application.SignOut;
using Dara.Shared.Contracts.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Apps.API.Modules.Identity;

// sign-in over http: the account panel with login and password (a session cookie), a new client with a registration
// code from the panel and an existing one with a login code from the panel (client tokens); the identity module keeps
// accounts, clients and sessions, the host issues the tokens and the cookie; accounts come from the seed for now
public static class AuthEndpoints
{
    // the rate limiter policy of register-client and client-login: a code has about 40 bits, a limit per address
    // keeps guessing slow
    public const string ClientCodeRateLimit = "client-code";

    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(AuthRoutes.Group).AddEndpointFilter<ClientProblems>();

        group.MapPost("/login", LoginAsync).AllowAnonymous();
        group.MapPost("/register-client", RegisterClientAsync).AllowAnonymous().RequireRateLimiting(ClientCodeRateLimit);
        group.MapPost("/client-login", ClientLoginAsync).AllowAnonymous().RequireRateLimiting(ClientCodeRateLimit);
        group.MapPost("/refresh", RefreshAsync).AllowAnonymous();
        // a client token or the panel cookie: the claims say which session ends
        group.MapPost("/logout", LogoutAsync).RequireAuthorization();

        // the other endpoints of the panel under /account/clients are in AccountClientsEndpoints
        var accountClients = app.MapGroup(AuthRoutes.AccountClientsGroup).RequireAuthorization(AuthPolicies.Account)
            .AddEndpointFilter<ClientProblems>();
        accountClients.MapPost("/registration-code", CreateRegistrationCodeAsync);
        accountClients.MapPost("/{clientId:guid}/login-code", IssueClientLoginCodeAsync);

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

    // a broken rule (a name too long) is 400 through ClientProblems
    private static async Task<IResult> RegisterClientAsync(RegisterClientRequest request, [FromServices] IIdentityModule identity, [FromServices] AccessTokenIssuer issuer)
    {
        ClientTokensResult result;
        try
        {
            result = await identity.ExecuteCommandAsync<RegisterClientCommand, ClientTokensResult>(
                new RegisterClientCommand(request.Code, request.Type, request.Name));
        }
        catch (DbUpdateConcurrencyException)
        {
            // another registration with the same code was saved first
            return Refused(AuthError.InvalidRegistrationCode);
        }

        return result.Succeeded ? TypedResults.Ok(Tokens(issuer, result)) : Refused(result.Error!.Value);
    }

    private static async Task<IResult> ClientLoginAsync(ClientLoginRequest request, [FromServices] IIdentityModule identity, [FromServices] AccessTokenIssuer issuer)
    {
        ClientTokensResult result;
        try
        {
            result = await identity.ExecuteCommandAsync<SignInClientCommand, ClientTokensResult>(new SignInClientCommand(request.Code));
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
        ClientTokensResult result;
        try
        {
            result = await identity.ExecuteCommandAsync<RefreshSessionCommand, ClientTokensResult>(new RefreshSessionCommand(request.RefreshToken));
        }
        catch (DbUpdateConcurrencyException)
        {
            // another refresh with the same token was saved first; this one is refused without ending the session
            return Refused(AuthError.RefreshInvalid);
        }

        return result.Succeeded ? TypedResults.Ok(Tokens(issuer, result)) : Refused(result.Error!.Value);
    }

    // a client: its refresh tokens end, the access token stays valid until it expires (at most 15 minutes);
    // the panel: its session ends at once and the cookie is removed
    private static async Task<IResult> LogoutAsync(ClaimsPrincipal user, HttpContext http, [FromServices] IIdentityModule identity)
    {
        if (user.IsClient())
            await identity.ExecuteCommandAsync(new SignOutClientCommand(user.UserId(), user.ClientId()));

        if (user.IsAccount())
        {
            await identity.ExecuteCommandAsync(new SignOutAccountCommand(user.UserId(), user.AccountSessionId()));
            await http.SignOutAsync(AccountCookie.Scheme);
        }

        return TypedResults.NoContent();
    }

    private static async Task<IResult> CreateRegistrationCodeAsync(ClaimsPrincipal user, [FromServices] IIdentityModule identity)
    {
        var result = await identity.ExecuteCommandAsync<CreateClientRegistrationCodeCommand, ClientCodeResult>(
            new CreateClientRegistrationCodeCommand(user.UserId()));

        return Code(result);
    }

    private static async Task<IResult> IssueClientLoginCodeAsync(Guid clientId, ClaimsPrincipal user, [FromServices] IIdentityModule identity)
    {
        var result = await identity.ExecuteCommandAsync<IssueClientLoginCodeCommand, ClientCodeResult>(
            new IssueClientLoginCodeCommand(user.UserId(), clientId));

        return Code(result);
    }

    private static IResult Code(ClientCodeResult result)
    {
        return result.Succeeded
            ? TypedResults.Ok(new ClientCodeResponse(result.Code!, result.ExpiresAt!.Value))
            : Refused(result.Error!.Value);
    }

    private static AuthTokensResponse Tokens(AccessTokenIssuer issuer, ClientTokensResult result)
    {
        var access = issuer.IssueClient(result.UserId, result.ClientId);
        return new AuthTokensResponse(access.Token, access.ExpiresAt, result.RefreshToken!, result.RefreshTokenExpiresAt!.Value,
            result.UserId, result.ClientId);
    }

    private static IResult Refused(AuthError error)
    {
        return error switch
        {
            AuthError.InvalidCredentials => ClientProblems.Problem(StatusCodes.Status401Unauthorized, AuthErrorCodes.InvalidCredentials, "Wrong login or password"),
            AuthError.InvalidRegistrationCode => ClientProblems.Problem(StatusCodes.Status401Unauthorized, AuthErrorCodes.InvalidRegistrationCode, "The registration code is not valid, get a new one in the account panel"),
            AuthError.InvalidLoginCode => ClientProblems.Problem(StatusCodes.Status401Unauthorized, AuthErrorCodes.InvalidLoginCode, "The code is not valid, get a new one in the account panel"),
            AuthError.ClientNotFound => ClientProblems.NotFound(),
            AuthError.ClientRevoked => ClientProblems.Revoked(),
            AuthError.RefreshExpired => ClientProblems.Problem(StatusCodes.Status401Unauthorized, AuthErrorCodes.RefreshExpired, "The refresh token expired, sign in with a new code"),
            AuthError.RefreshReused => ClientProblems.Problem(StatusCodes.Status401Unauthorized, AuthErrorCodes.RefreshReused, "The refresh token was used before, the session is ended, sign in with a new code"),
            AuthError.RefreshRevoked => ClientProblems.Problem(StatusCodes.Status401Unauthorized, AuthErrorCodes.RefreshRevoked, "The session is ended, sign in with a new code"),
            _ => ClientProblems.Problem(StatusCodes.Status401Unauthorized, AuthErrorCodes.RefreshInvalid, "The refresh token is not valid, sign in with a new code")
        };
    }
}
