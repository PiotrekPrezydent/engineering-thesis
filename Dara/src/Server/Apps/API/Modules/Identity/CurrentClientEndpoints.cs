using System.Security.Claims;
using Dara.Server.Apps.API.Common.Authentication;
using Dara.Server.Modules.Identity.Application;
using Dara.Server.Modules.Identity.Application.Clients;
using Dara.Server.Modules.Identity.Application.GetClients;
using Dara.Shared.Contracts.Clients;
using Microsoft.AspNetCore.Mvc;

namespace Dara.Server.Apps.API.Modules.Identity;

// the client token: the client reads itself (name, type, context); it changes nothing here
public static class CurrentClientEndpoints
{
    public static IEndpointRouteBuilder MapCurrentClientEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(ClientRoutes.Current)
            .RequireAuthorization(AuthPolicies.Client)
            .AddEndpointFilter<ClientProblems>();

        group.MapGet("/", GetAsync);

        return app;
    }

    private static async Task<IResult> GetAsync(ClaimsPrincipal user, [FromServices] IIdentityModule identity)
    {
        var client = await identity.ExecuteQueryAsync<GetCurrentClientQuery, UserClientDto?>(new GetCurrentClientQuery(user.ClientId()));
        return client is null ? ClientProblems.NotFound() : TypedResults.Ok(client.ToSnapshot());
    }
}
