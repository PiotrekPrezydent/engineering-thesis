using System.Security.Claims;
using Dara.Server.Apps.API.Common.Authentication;
using Dara.Server.Apps.API.Common.Authorization;
using Dara.Server.Modules.Authorization.Application;
using Dara.Server.Modules.Authorization.Application.Definitions.GetRoles;
using Dara.Server.Modules.Authorization.Application.RoleAssignments;
using Dara.Server.Modules.Authorization.Application.RoleAssignments.GetMyAssignments;
using Dara.Server.Modules.Authorization.Application.RoleAssignments.GrantRole;
using Dara.Server.Modules.Authorization.Application.RoleAssignments.RevokeRole;
using Dara.Shared.Contracts.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dara.Server.Apps.API.Modules.Authorization;

// anyone with a session calls them, a client by its token or a user by the panel cookie, as itself (the sender);
// who may grant and revoke decides the module of the scope (the permission that manages access, the manageable
// roles), checked by the rules of Authorization (403)
public static class AuthorizationEndpoints
{
    public static IEndpointRouteBuilder MapAuthorizationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(AuthorizationRoutes.Group)
            .RequireAuthorization()
            .AddEndpointFilter<AuthorizationProblems>();

        group.MapGet("/roles", GetRolesAsync);
        group.MapGet("/me/assignments", GetMyAssignmentsAsync);
        group.MapPost("/assignments", GrantAsync);
        group.MapPost("/assignments/revoke", RevokeAsync);

        return app;
    }

    private static async Task<IResult> GetRolesAsync([FromServices] IAuthorizationModule authorization)
    {
        var roles = await authorization.ExecuteQueryAsync<GetRolesQuery, List<RoleDto>>(new GetRolesQuery());
        return TypedResults.Ok(roles.Select(r => new RoleSnapshot(r.Code, r.ScopeCode, r.Module, r.PermissionCodes, r.ManageableRoleCodes, r.CompanionRoleCodes)).ToList());
    }

    private static async Task<IResult> GetMyAssignmentsAsync(ClaimsPrincipal user, [FromServices] IAuthorizationModule authorization)
    {
        var assignments = await authorization.ExecuteQueryAsync<GetMyAssignmentsQuery, List<RoleAssignmentDto>>(
            new GetMyAssignmentsQuery(user.SenderId()));
        return TypedResults.Ok(assignments
            .Select(a => new RoleAssignmentSnapshot(a.SubjectId, a.RoleCode, a.ScopeCode, a.ScopeId, a.PermissionCodes))
            .ToList());
    }

    private static async Task<IResult> GrantAsync(ClaimsPrincipal user, RoleAssignmentRequest request, [FromServices] IAuthorizationModule authorization)
    {
        await authorization.ExecuteCommandAsync(new GrantRoleCommand(user.SenderId(), request.SubjectId, request.RoleCode, request.ScopeId,
            CompanionsOf(request)));
        return TypedResults.NoContent();
    }

    private static async Task<IResult> RevokeAsync(ClaimsPrincipal user, RoleAssignmentRequest request, [FromServices] IAuthorizationModule authorization)
    {
        await authorization.ExecuteCommandAsync(new RevokeRoleCommand(user.SenderId(), request.SubjectId, request.RoleCode, request.ScopeId,
            CompanionsOf(request)));
        return TypedResults.NoContent();
    }

    private static List<CompanionAssignment> CompanionsOf(RoleAssignmentRequest request)
    {
        return (request.Companions ?? []).Select(c => new CompanionAssignment(c.RoleCode, c.ScopeId)).ToList();
    }
}
