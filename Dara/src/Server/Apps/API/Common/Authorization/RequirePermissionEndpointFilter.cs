using Dara.Server.BuildingBlocks.Application.Authorization;
using Dara.Server.Modules.Authorization.Application;
using Dara.Shared.Contracts.Authorization;

namespace Dara.Server.Apps.API.Common.Authorization;

// before the handler: the id of the resource from the route (400 when it is no guid), then the permission of the
// sender on that resource (403 when no assignment allows it)
public sealed class RequirePermissionEndpointFilter : IEndpointFilter
{
    private readonly PermissionDefinition _permission;
    private readonly string _scopeIdRouteValue;

    public RequirePermissionEndpointFilter(PermissionDefinition permission, string scopeIdRouteValue)
    {
        _permission = permission;
        _scopeIdRouteValue = scopeIdRouteValue;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var http = context.HttpContext;
        if (!Guid.TryParse(http.Request.RouteValues[_scopeIdRouteValue]?.ToString(), out var scopeId))
            return AuthorizationProblems.Problem(StatusCodes.Status400BadRequest, AuthorizationErrorCodes.InvalidScopeId,
                $"route value {_scopeIdRouteValue} is not the id of a resource");

        var authorization = http.RequestServices.GetRequiredService<IAuthorizationModule>();
        if (!await PermissionGate.IsAllowedAsync(http.User, authorization, _permission, scopeId))
            return AuthorizationProblems.Forbidden($"the caller may not {_permission.Code} on this resource");

        return await next(context);
    }
}

public static class RequirePermissionExtensions
{
    // app.MapPost("/groups/{groupId}/messages", ...).RequirePermission(GroupsPermissions.SendMessage, "groupId");
    // an endpoint that needs several permissions gets several marks, each is checked in order
    public static TBuilder RequirePermission<TBuilder>(this TBuilder builder, PermissionDefinition permission, string scopeIdRouteValue)
        where TBuilder : IEndpointConventionBuilder
    {
        return builder.AddEndpointFilter(new RequirePermissionEndpointFilter(permission, scopeIdRouteValue));
    }
}
