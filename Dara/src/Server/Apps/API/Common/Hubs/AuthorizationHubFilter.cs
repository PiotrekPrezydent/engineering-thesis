using System.Reflection;
using Dara.Server.Apps.API.Common.Authorization;
using Dara.Server.Modules.Authorization.Application;
using Dara.Shared.Contracts.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Dara.Server.Apps.API.Common.Hubs;

// hub methods marked with RequirePermission run only when, for every mark, an assignment of the sender allows the
// permission on the resource of the marked argument; otherwise the caller gets a HubException "forbidden"
public class AuthorizationHubFilter : IHubFilter
{
    public async ValueTask<object?> InvokeMethodAsync(HubInvocationContext invocationContext, Func<HubInvocationContext, ValueTask<object?>> next)
    {
        var attributes = invocationContext.HubMethod.GetCustomAttributes<RequirePermissionAttribute>().ToList();
        if (attributes.Count == 0)
            return await next(invocationContext);

        var authorization = invocationContext.ServiceProvider.GetRequiredService<IAuthorizationModule>();
        foreach (var attribute in attributes)
        {
            if (!await PermissionGate.IsAllowedAsync(invocationContext.Context.User!, authorization, attribute.Permission, ScopeIdOf(invocationContext, attribute)))
                throw new HubException(AuthorizationErrorCodes.Forbidden);
        }

        return await next(invocationContext);
    }

    private static Guid ScopeIdOf(HubInvocationContext invocationContext, RequirePermissionAttribute attribute)
    {
        var index = Array.FindIndex(invocationContext.HubMethod.GetParameters(), p => p.Name == attribute.ScopeIdArgument);
        if (index < 0)
            throw new InvalidOperationException($"{invocationContext.HubMethodName} has no argument {attribute.ScopeIdArgument}");

        return invocationContext.HubMethodArguments[index] switch
        {
            Guid id => id,
            string text when Guid.TryParse(text, out var parsed) => parsed,
            _ => throw new HubException($"{attribute.ScopeIdArgument} is not the id of a resource")
        };
    }
}
