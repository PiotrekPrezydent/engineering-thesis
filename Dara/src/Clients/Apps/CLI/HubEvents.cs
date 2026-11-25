using Dara.Shared.Contracts;
using Dara.Shared.Contracts.Notifications;
using Dara.Shared.Contracts.StateSnapshots;

namespace Dara.Clients.Apps.CLI;

public class HubEvents : IAppHubClient
{
    public async Task OnProfileNameChanged(ProfileNameChangedNotification notification)
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