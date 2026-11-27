using System.Security.Claims;
using Dara.Server.Apps.API.Common.Authentication;
using Dara.Server.Modules.Identity.Application;
using Dara.Server.Modules.Identity.Application.ChangeClientContext;
using Dara.Server.Modules.Identity.Application.Clients;
using Dara.Server.Modules.Identity.Application.GetClients;
using Dara.Server.Modules.Identity.Application.RenameClient;
using Dara.Server.Modules.Identity.Application.RevokeClient;
using Dara.Shared.Contracts.Clients;
using Microsoft.AspNetCore.Mvc;

namespace Dara.Server.Apps.API.Modules.Identity;

// the account panel manages the clients of its account (the cookie of the panel); a client registers itself with a
// code of the panel, so there is no create here; the codes are in AuthEndpoints under the same prefix
public static class AccountClientsEndpoints
{
    public static IEndpointRouteBuilder MapAccountClientsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(ClientRoutes.AccountClients)
            .RequireAuthorization(AuthPolicies.Account)
            .AddEndpointFilter<ClientProblems>();

        group.MapGet("/", GetAllAsync);
        group.MapGet("/{clientId:guid}", GetAsync);
        group.MapPut("/{clientId:guid}/name", RenameAsync);
        group.MapPut("/{clientId:guid}/context", ChangeContextAsync);
        group.MapPost("/{clientId:guid}/revoke", RevokeAsync);

        return app;
    }

    private static async Task<IResult> GetAllAsync(ClaimsPrincipal user, [FromServices] IIdentityModule identity)
    {
        var all = await identity.ExecuteQueryAsync<GetAccountClientsQuery, List<UserClientDto>>(new GetAccountClientsQuery(user.UserId()));
        return TypedResults.Ok(all.Select(c => c.ToSnapshot()).ToList());
    }

    private static async Task<IResult> GetAsync(Guid clientId, ClaimsPrincipal user, [FromServices] IIdentityModule identity)
    {
        var client = await identity.ExecuteQueryAsync<GetAccountClientQuery, UserClientDto?>(new GetAccountClientQuery(user.UserId(), clientId));
        return client is null ? ClientProblems.NotFound() : TypedResults.Ok(client.ToSnapshot());
    }

    private static async Task<IResult> RenameAsync(Guid clientId, RenameClientRequest request, ClaimsPrincipal user, [FromServices] IIdentityModule identity)
    {
        await identity.ExecuteCommandAsync(new RenameClientCommand(user.UserId(), clientId, request.Name));
        return TypedResults.NoContent();
    }

    private static async Task<IResult> ChangeContextAsync(Guid clientId, ClientContextRequest request, ClaimsPrincipal user, [FromServices] IIdentityModule identity)
    {
        await identity.ExecuteCommandAsync(new ChangeClientContextCommand(user.UserId(), clientId, request.Entries ?? new Dictionary<string, string>()));
        return TypedResults.NoContent();
    }

    // the session of the client ends at once; the host closes its hub connections on UserClientRevoked
    private static async Task<IResult> RevokeAsync(Guid clientId, ClaimsPrincipal user, [FromServices] IIdentityModule identity)
    {
        await identity.ExecuteCommandAsync(new RevokeClientCommand(user.UserId(), clientId));
        return TypedResults.NoContent();
    }
}
