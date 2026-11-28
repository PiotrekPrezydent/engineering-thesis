using Dara.Clients.Core.Api;
using Dara.Shared.Contracts;
using Dara.Shared.Contracts.FunctionCalls;
using Dara.Shared.Contracts.Notifications;
using Dara.Shared.Contracts.StateSnapshots;

namespace Dara.Clients.Apps.CLI;

public class HubEvents : IAppHubClient
{
    private readonly DaraApiClient _api;

    public HubEvents(DaraApiClient api)
    {
        _api = api;
    }

    public async Task OnClientPluginsChanged(ClientPluginsChangedNotification notification)
    {
        Console.WriteLine(notification);
    }

    // the cli runs no plugins (TASK-ENG-034), so it answers at once and the step does not wait for its time limit
    public async Task CallFunction(FunctionCallRequest request)
    {
        Console.WriteLine($"call {request.CallId}: {request.PluginName}.{request.FunctionName} {request.ArgumentsJson}");
        try
        {
            await _api.SubmitFunctionCallResultAsync(request.CallId, new FunctionCallResultRequest(FunctionCallOutcome.NotExecuted, null, "not supported by the cli"));
        }
        catch (Exception e) when (e is DaraApiException or HttpRequestException)
        {
            Console.WriteLine($"error: {e.Message}");
        }
    }

    public async Task OnClientRenamed(ClientRenamedNotification notification)
    {
        Console.WriteLine(notification);
    }

    public async Task OnClientRevoked(ClientRevokedNotification notification)
    {
        Console.WriteLine(notification);
    }

    public async Task OnGroupCreated(GroupCreatedNotification notification)
    {
        Console.WriteLine(notification);
    }

    public async Task OnGroupMemberUpdated(GroupMemberSnapshot groupMemberSnapshot)
    {
        Console.WriteLine(groupMemberSnapshot);
    }

    public async Task OnGroupJoined(GroupSnapshot groupSnapshot)
    {
        Console.WriteLine(groupSnapshot);
    }

    public async Task OnGroupLeft(Guid groupId)
    {
        Console.WriteLine(groupId);
    }

    public async Task OnGroupMemberJoined(GroupMemberJoinedGroupNotification notification)
    {
        Console.WriteLine(notification);
    }

    public async Task OnGroupMemberLeft(GroupMemberLeftGroupNotification notification)
    {
        Console.WriteLine(notification);
    }

    public async Task OnGroupMessageReceived(GroupMessageReceivedNotification notification)
    {
        Console.WriteLine(notification);
    }

    public async Task OnPluginsRegistered(PluginsRegisteredNotification notification)
    {
        Console.WriteLine(nameof(OnPluginsRegistered));
        PrintPlugins(notification.Plugins);
    }

    public async Task OnAgentRunStarted(AgentRunStartedNotification notification)
    {
        Console.WriteLine(notification);
    }

    public async Task OnExecutionPlanCreated(ExecutionPlanCreatedNotification notification)
    {
        Console.WriteLine($"plan of agent run {notification.AgentRunId} in group {notification.GroupId}:");
        PrintPlan(notification.AgentRunId, notification.Calls);
    }

    public async Task OnExecutionPlanGenerationFailed(ExecutionPlanGenerationFailedNotification notification)
    {
        Console.WriteLine(notification);
    }

    public async Task OnExecutionPlanApproved(ExecutionPlanApprovedNotification notification)
    {
        Console.WriteLine(notification);
    }

    public async Task OnAgentRunRejected(AgentRunRejectedNotification notification)
    {
        Console.WriteLine(notification);
    }

    public static void PrintPlugins(IReadOnlyList<PluginSnapshot> plugins)
    {
        foreach (var plugin in plugins)
        {
            Console.WriteLine($"plugin {plugin.PluginId} {plugin.Name} {plugin.Version} enabled: {plugin.IsEnabled}");
            foreach (var function in plugin.Functions)
                Console.WriteLine($"  function {function.FunctionId} {function.Description.Name} enabled: {function.IsEnabled}");
        }
    }

    public static void PrintPlan(Guid agentRunId, IReadOnlyList<PlannedCallSnapshot> calls)
    {
        foreach (var call in calls)
            Console.WriteLine($"  {call.Index}. {call.ParticipantName}.{call.FunctionName} {call.ArgumentsJson}");

        Console.WriteLine($"approve: ap {agentRunId}, reject: rj {agentRunId}");
    }
}