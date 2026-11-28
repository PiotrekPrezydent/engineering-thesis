using Dara.Server.Apps.API.Common.Authentication;
using Dara.Shared.Contracts.Account;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.Extensions.Options;

namespace Dara.Server.Apps.API.Common.Endpoints;

// the panel takes a token here for its forms (the upload of a plugin package, TASK-ENG-031); the token is bound to the
// identity of the cookie, so after a new sign-in the panel takes a new one. Json endpoints of the panel need none
public static class AccountAntiforgeryEndpoints
{
    public static IEndpointRouteBuilder MapAccountAntiforgeryEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet(AccountRoutes.Antiforgery, (HttpContext context, IAntiforgery antiforgery, IOptions<AntiforgeryOptions> options) =>
        {
            var tokens = antiforgery.GetAndStoreTokens(context);
            return TypedResults.Ok(new AntiforgeryTokenResponse(tokens.RequestToken!, options.Value.FormFieldName, options.Value.HeaderName));
        }).RequireAuthorization(AuthPolicies.Account);

        return app;
    }
}
