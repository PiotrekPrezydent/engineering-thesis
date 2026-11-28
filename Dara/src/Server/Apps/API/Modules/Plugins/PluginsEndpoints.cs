using System.Security.Claims;
using Dara.Server.Apps.API.Common.Authentication;
using Dara.Server.Apps.API.Common.Authorization;
using Dara.Server.Apps.API.Common.Errors;
using Dara.Server.Modules.Plugins.Application;
using Dara.Server.Modules.Plugins.Application.PluginConfigurations.DisablePlugin;
using Dara.Server.Modules.Plugins.Application.PluginConfigurations.DisablePluginFunction;
using Dara.Server.Modules.Plugins.Application.PluginConfigurations.EnablePlugin;
using Dara.Server.Modules.Plugins.Application.PluginConfigurations.EnablePluginFunction;
using Dara.Server.Modules.Plugins.Application.Plugins.GetOwnerPlugins;
using Dara.Server.Modules.Plugins.Application.Plugins.RegisterPlugins;
using Dara.Shared.Contracts.Auth;
using Dara.Shared.Contracts.Plugins;
using Microsoft.AspNetCore.Mvc;

namespace Dara.Server.Apps.API.Modules.Plugins;

// the client reads and reports its own set of plugins (client token); the panel reads and configures the plugins of a
// client of its account (panel cookie). TASK-ENG-031 replaces both with collections and installations
public static class PluginsEndpoints
{
    public static IEndpointRouteBuilder MapPluginsEndpoints(this IEndpointRouteBuilder app)
    {
        var current = app.MapGroup(PluginRoutes.CurrentClientPlugins)
            .RequireAuthorization(AuthPolicies.Client)
            .AddEndpointFilter<DomainProblems>();
        current.MapGet("/", GetCurrentAsync);
        current.MapPut("/", RegisterAsync);

        var account = app.MapGroup(AuthRoutes.AccountClientsGroup + "/{clientId:guid}/plugins")
            .RequireAuthorization(AuthPolicies.Account)
            .AddEndpointFilter<DomainProblems>();
        account.MapGet("/", GetOfClientAsync);
        account.MapPost("/{pluginId:guid}/enable", EnableAsync);
        account.MapPost("/{pluginId:guid}/disable", DisableAsync);
        account.MapPost("/{pluginId:guid}/functions/{functionId:guid}/enable", EnableFunctionAsync);
        account.MapPost("/{pluginId:guid}/functions/{functionId:guid}/disable", DisableFunctionAsync);

        return app;
    }

    private static async Task<IResult> GetCurrentAsync(ClaimsPrincipal user, [FromServices] IPluginsModule plugins)
    {
        return TypedResults.Ok(await PluginsOfAsync(plugins, user.ClientId()));
    }

    // the whole set replaces the registered one (TASK-ENG-031: the client reports the state of its installations)
    private static async Task<IResult> RegisterAsync(RegisterPluginsRequest request, ClaimsPrincipal user, [FromServices] IPluginsModule plugins)
    {
        await plugins.ExecuteCommandAsync(new RegisterPluginsCommand(user.ClientId(), request.Plugins ?? []));
        return TypedResults.NoContent();
    }

    private static async Task<IResult> GetOfClientAsync(Guid clientId, ClaimsPrincipal user, [FromServices] ActingClientResolver acting,
        [FromServices] IPluginsModule plugins)
    {
        var owner = await acting.FromPanelAsync(user, clientId);
        return TypedResults.Ok(await PluginsOfAsync(plugins, owner.ClientId));
    }

    private static async Task<IResult> EnableAsync(Guid clientId, Guid pluginId, ClaimsPrincipal user, [FromServices] ActingClientResolver acting,
        [FromServices] IPluginsModule plugins)
    {
        var owner = await acting.FromPanelAsync(user, clientId);
        await plugins.ExecuteCommandAsync(new EnablePluginCommand(owner.ClientId, pluginId));
        return TypedResults.NoContent();
    }

    private static async Task<IResult> DisableAsync(Guid clientId, Guid pluginId, ClaimsPrincipal user, [FromServices] ActingClientResolver acting,
        [FromServices] IPluginsModule plugins)
    {
        var owner = await acting.FromPanelAsync(user, clientId);
        await plugins.ExecuteCommandAsync(new DisablePluginCommand(owner.ClientId, pluginId));
        return TypedResults.NoContent();
    }

    private static async Task<IResult> EnableFunctionAsync(Guid clientId, Guid pluginId, Guid functionId, ClaimsPrincipal user,
        [FromServices] ActingClientResolver acting, [FromServices] IPluginsModule plugins)
    {
        var owner = await acting.FromPanelAsync(user, clientId);
        await plugins.ExecuteCommandAsync(new EnablePluginFunctionCommand(owner.ClientId, pluginId, functionId));
        return TypedResults.NoContent();
    }

    private static async Task<IResult> DisableFunctionAsync(Guid clientId, Guid pluginId, Guid functionId, ClaimsPrincipal user,
        [FromServices] ActingClientResolver acting, [FromServices] IPluginsModule plugins)
    {
        var owner = await acting.FromPanelAsync(user, clientId);
        await plugins.ExecuteCommandAsync(new DisablePluginFunctionCommand(owner.ClientId, pluginId, functionId));
        return TypedResults.NoContent();
    }

    private static async Task<List<Shared.Contracts.StateSnapshots.PluginSnapshot>> PluginsOfAsync(IPluginsModule plugins, Guid clientId)
    {
        var owned = await plugins.ExecuteQueryAsync<GetOwnerPluginsQuery, List<OwnerPluginDto>>(new GetOwnerPluginsQuery(clientId));
        return owned.Select(p => p.ToSnapshot()).ToList();
    }
}
