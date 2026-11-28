using System.Security.Claims;
using Dara.Server.Apps.API.Common.Authentication;
using Dara.Server.Apps.API.Common.Errors;
using Dara.Server.Modules.Orchestration.Application;
using Dara.Server.Modules.Orchestration.Application.AgentRuns.ApproveExecutionPlan;
using Dara.Server.Modules.Orchestration.Application.AgentRuns.GetAgentRun;
using Dara.Server.Modules.Orchestration.Application.AgentRuns.RejectExecutionPlan;
using Dara.Server.Modules.Orchestration.Application.Groups.EditApiKey;
using Dara.Server.Modules.Orchestration.Application.Groups.EditChatClientProviderName;
using Dara.Server.Modules.Orchestration.Application.Groups.EditClientModelName;
using Dara.Server.Modules.Orchestration.Application.Groups.EditPromptMessagePrefix;
using Dara.Server.Modules.Orchestration.Application.Groups.GetGroupAgentSettings;
using Dara.Shared.Contracts.Groups;
using Dara.Shared.Contracts.Orchestration;
using Microsoft.AspNetCore.Mvc;

namespace Dara.Server.Apps.API.Modules.Orchestration;

// the author of a prompt reads and decides its agent run (client token, until TASK-ENG-013 accepts every call); the
// panel of the owner of a group reads and edits its agent settings (panel cookie, as the owner client of the group
// until TASK-ENG-030)
public static class OrchestrationEndpoints
{
    public static IEndpointRouteBuilder MapOrchestrationEndpoints(this IEndpointRouteBuilder app)
    {
        var runs = app.MapGroup(AgentRunRoutes.AgentRuns)
            .RequireAuthorization(AuthPolicies.Client)
            .AddEndpointFilter<DomainProblems>();
        runs.MapGet("/{agentRunId:guid}", GetAgentRunAsync);
        runs.MapPost("/{agentRunId:guid}/approve", ApproveAsync);
        runs.MapPost("/{agentRunId:guid}/reject", RejectAsync);

        var settings = app.MapGroup(GroupRoutes.AccountGroups + "/{groupId:guid}/agent-settings")
            .RequireAuthorization(AuthPolicies.Account)
            .AddEndpointFilter<DomainProblems>();
        settings.MapGet("/", GetSettingsAsync);
        settings.MapPut("/prompt-prefix", EditPromptPrefixAsync);
        settings.MapPut("/provider", EditProviderAsync);
        settings.MapPut("/model", EditModelAsync);
        settings.MapPut("/api-key", EditApiKeyAsync);

        return app;
    }

    // another client than the author gets 404, as for a missing run
    private static async Task<IResult> GetAgentRunAsync(Guid agentRunId, ClaimsPrincipal user, [FromServices] IOrchestrationModule orchestration)
    {
        var run = await orchestration.ExecuteQueryAsync<GetAgentRunQuery, AgentRunDto?>(new GetAgentRunQuery(agentRunId, user.ClientId()));
        return run is null ? DomainProblems.NotFound($"AgentRun {agentRunId} was not found") : TypedResults.Ok(run.ToSnapshot());
    }

    private static async Task<IResult> ApproveAsync(Guid agentRunId, ClaimsPrincipal user, [FromServices] IOrchestrationModule orchestration)
    {
        await orchestration.ExecuteCommandAsync(new ApproveExecutionPlanCommand(agentRunId, user.ClientId()));
        return TypedResults.NoContent();
    }

    private static async Task<IResult> RejectAsync(Guid agentRunId, ClaimsPrincipal user, [FromServices] IOrchestrationModule orchestration)
    {
        await orchestration.ExecuteCommandAsync(new RejectExecutionPlanCommand(agentRunId, user.ClientId()));
        return TypedResults.NoContent();
    }

    private static async Task<IResult> GetSettingsAsync(Guid groupId, ClaimsPrincipal user, [FromServices] GroupOwnerResolver owners,
        [FromServices] IOrchestrationModule orchestration)
    {
        var owner = await owners.OwnerClientOfAsync(user, groupId);
        var settings = await orchestration.ExecuteQueryAsync<GetGroupAgentSettingsQuery, GroupAgentSettingsDto?>(new GetGroupAgentSettingsQuery(groupId, owner));
        return settings is null ? DomainProblems.NotFound($"Group {groupId} was not found") : TypedResults.Ok(settings.ToSnapshot());
    }

    private static async Task<IResult> EditPromptPrefixAsync(Guid groupId, EditPromptMessagePrefixRequest request, ClaimsPrincipal user,
        [FromServices] GroupOwnerResolver owners, [FromServices] IOrchestrationModule orchestration)
    {
        var owner = await owners.OwnerClientOfAsync(user, groupId);
        await orchestration.ExecuteCommandAsync(new EditPromptMessagePrefixCommand(groupId, owner, request.PromptMessagePrefix));
        return TypedResults.NoContent();
    }

    private static async Task<IResult> EditProviderAsync(Guid groupId, EditChatClientProviderRequest request, ClaimsPrincipal user,
        [FromServices] GroupOwnerResolver owners, [FromServices] IOrchestrationModule orchestration)
    {
        var owner = await owners.OwnerClientOfAsync(user, groupId);
        await orchestration.ExecuteCommandAsync(new EditChatClientProviderNameCommand(groupId, owner, request.ChatClientProviderName));
        return TypedResults.NoContent();
    }

    private static async Task<IResult> EditModelAsync(Guid groupId, EditClientModelRequest request, ClaimsPrincipal user,
        [FromServices] GroupOwnerResolver owners, [FromServices] IOrchestrationModule orchestration)
    {
        var owner = await owners.OwnerClientOfAsync(user, groupId);
        await orchestration.ExecuteCommandAsync(new EditClientModelNameCommand(groupId, owner, request.ClientModelName));
        return TypedResults.NoContent();
    }

    private static async Task<IResult> EditApiKeyAsync(Guid groupId, EditApiKeyRequest request, ClaimsPrincipal user,
        [FromServices] GroupOwnerResolver owners, [FromServices] IOrchestrationModule orchestration)
    {
        var owner = await owners.OwnerClientOfAsync(user, groupId);
        await orchestration.ExecuteCommandAsync(new EditApiKeyCommand(groupId, owner, request.ApiKey));
        return TypedResults.NoContent();
    }
}
