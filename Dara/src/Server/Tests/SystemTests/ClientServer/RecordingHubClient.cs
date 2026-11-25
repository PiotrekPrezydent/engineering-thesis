using Dara.Server.SystemTests.Hosting;
using Dara.Shared.Contracts;
using Dara.Shared.Contracts.Notifications;
using Dara.Shared.Contracts.StateSnapshots;

namespace Dara.Server.SystemTests.ClientServer;

// keeps every notification the server sends to one device, signalr calls it on its own threads
public class RecordingHubClient : IAppHubClient
{
    private readonly Lock _lock = new();
    private readonly List<object> _received = new();

    public IReadOnlyList<T> Received<T>()
    {
        lock (_lock)
            return _received.OfType<T>().ToList();
    }

    public async Task<T> WaitForAsync<T>(Func<T, bool> match, string description)
    {
        T? found = default;
        await ModulesHost.WaitUntilAsync(() =>
        {
            found = Received<T>().FirstOrDefault(match);
            return Task.FromResult(found is not null);
        }, description);

        return found!;
    }

    public Task OnProfileNameChanged(ProfileNameChangedNotification notification)
    {
        return Record(notification);
    }

    public Task OnGroupCreated(GroupCreatedNotification notification)
    {
        return Record(notification);
    }

    public Task OnGroupMemberUpdated(GroupMemberSnapshot groupMemberSnapshot)
    {
        return Record(groupMemberSnapshot);
    }

    public Task OnGroupJoined(GroupSnapshot groupSnapshot)
    {
        return Record(groupSnapshot);
    }

    public Task OnGroupLeft(Guid groupId)
    {
        return Record(new GroupLeft(groupId));
    }

    public Task OnGroupMemberJoined(GroupMemberJoinedGroupNotification notification)
    {
        return Record(notification);
    }

    public Task OnGroupMemberLeft(GroupMemberLeftGroupNotification notification)
    {
        return Record(notification);
    }

    public Task OnGroupMessageReceived(GroupMessageReceivedNotification notification)
    {
        return Record(notification);
    }

    public Task OnPluginsRegistered(PluginsRegisteredNotification notification)
    {
        return Record(notification);
    }

    public Task OnAgentRunStarted(AgentRunStartedNotification notification)
    {
        return Record(notification);
    }

    public Task OnExecutionPlanCreated(ExecutionPlanCreatedNotification notification)
    {
        return Record(notification);
    }

    public Task OnExecutionPlanGenerationFailed(ExecutionPlanGenerationFailedNotification notification)
    {
        return Record(notification);
    }

    public Task OnExecutionPlanApproved(ExecutionPlanApprovedNotification notification)
    {
        return Record(notification);
    }

    public Task OnAgentRunRejected(AgentRunRejectedNotification notification)
    {
        return Record(notification);
    }

    private Task Record(object notification)
    {
        lock (_lock)
            _received.Add(notification);

        return Task.CompletedTask;
    }
}

public record GroupLeft(Guid GroupId);
