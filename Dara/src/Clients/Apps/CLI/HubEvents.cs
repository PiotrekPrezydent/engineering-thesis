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

    public async Task OnPluginRegistered()
    {
        Console.WriteLine(nameof(OnPluginRegistered));
    }
}