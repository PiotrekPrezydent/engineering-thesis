using System.Security.Claims;
using Dara.Server.Apps.API.Common.Authentication;
using Dara.Server.Apps.API.Common.Authorization;
using Dara.Server.Apps.API.Common.Errors;
using Dara.Server.Modules.Groups.Application;
using Dara.Server.Modules.Groups.Application.Groups.CreateNewGroup;
using Dara.Server.Modules.Groups.Application.Groups.JoinToGroup;
using Dara.Server.Modules.Groups.Application.Groups.LeaveGroup;
using Dara.Server.Modules.Groups.Application.Messages.SendGroupMessage;
using Dara.Shared.Contracts.Auth;
using Dara.Shared.Contracts.Groups;
using Microsoft.AspNetCore.Mvc;

namespace Dara.Server.Apps.API.Modules.Groups;

// the client writes in its groups (client token); the panel creates groups and moves a client of its account in and
// out of them (panel cookie). Until TASK-ENG-030 a group is owned by a client and joined with its code
public static class GroupsEndpoints
{
    public static IEndpointRouteBuilder MapGroupsEndpoints(this IEndpointRouteBuilder app)
    {
        var client = app.MapGroup(GroupRoutes.ClientGroups)
            .RequireAuthorization(AuthPolicies.Client)
            .AddEndpointFilter<DomainProblems>();
        client.MapPost("/{groupId:guid}/messages", SendMessageAsync);

        var account = app.MapGroup(GroupRoutes.AccountGroups)
            .RequireAuthorization(AuthPolicies.Account)
            .AddEndpointFilter<DomainProblems>();
        account.MapPost("/", CreateAsync);

        var accountClient = app.MapGroup(AuthRoutes.AccountClientsGroup + "/{clientId:guid}/groups")
            .RequireAuthorization(AuthPolicies.Account)
            .AddEndpointFilter<DomainProblems>();
        accountClient.MapPost("/join", JoinAsync);
        accountClient.MapDelete("/{groupId:guid}", LeaveAsync);

        return app;
    }

    private static async Task<IResult> SendMessageAsync(Guid groupId, SendGroupMessageRequest request, ClaimsPrincipal user, [FromServices] IGroupsModule groups)
    {
        await groups.ExecuteCommandAsync(new SendGroupMessageCommand(groupId, user.ClientId(), request.Content));
        return TypedResults.NoContent();
    }

    // the owner client gets OnGroupCreated with the code too; TASK-ENG-030 makes the account the owner and the code real
    private static async Task<IResult> CreateAsync(CreateGroupRequest request, ClaimsPrincipal user, [FromServices] ActingClientResolver acting,
        [FromServices] IGroupsModule groups)
    {
        var owner = await acting.FromPanelAsync(user, request.ClientId);
        var joinCode = "DEFAULT-CODE" + Random.Shared.Next();
        var groupId = await groups.ExecuteCommandAsync<CreateNewGroupCommand, Guid>(new CreateNewGroupCommand(owner.ClientId, request.Name, joinCode));

        return TypedResults.Created($"{GroupRoutes.AccountGroups}/{groupId}", new CreateGroupResponse(groupId, joinCode));
    }

    private static async Task<IResult> JoinAsync(Guid clientId, JoinGroupRequest request, ClaimsPrincipal user, [FromServices] ActingClientResolver acting,
        [FromServices] IGroupsModule groups)
    {
        var member = await acting.FromPanelAsync(user, clientId);
        await groups.ExecuteCommandAsync(new JoinToGroupCommand(request.GroupId, member.ClientId, request.JoinCode));
        return TypedResults.NoContent();
    }

    private static async Task<IResult> LeaveAsync(Guid clientId, Guid groupId, ClaimsPrincipal user, [FromServices] ActingClientResolver acting,
        [FromServices] IGroupsModule groups)
    {
        var member = await acting.FromPanelAsync(user, clientId);
        await groups.ExecuteCommandAsync(new LeaveGroupCommand(groupId, member.ClientId));
        return TypedResults.NoContent();
    }
}
