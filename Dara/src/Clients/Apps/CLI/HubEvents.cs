using Dara.Shared.Contracts;

namespace Dara.Clients.Apps.CLI;

public class HubEvents : IAppHubClient
{
    public async Task OnProfileNameChanged(string newName)
    {
        Console.WriteLine("ON PROFILE NAME CHANGED + " +newName);
    }

    public async Task OnGroupCreated(Guid groupId, string groupName, string joinCode, List<Guid> memberIds)
    {
        Console.WriteLine("ON GROUP CREATED - " + groupId + " name: " +groupName + " jc: " +joinCode + " members: " +string.Join(", ",memberIds));
    }

    public async Task OnGroupMemberUpdated(Guid memberId)
    {
        Console.WriteLine("ON GROUP MEMBER UPDATED  "  + memberId);
    }

    public async Task OnGroupJoined(Guid groupId, string groupName, string joinCode, List<Guid> memberIds)
    {
        Console.WriteLine("ON GROUP JOINED  "  + groupId + " name: " +groupName + " jc: " +joinCode + " members: " +string.Join(", ",memberIds));
    }

    public async Task OnGroupLeft(Guid groupId)
    {
        Console.WriteLine("On GROUP LEFT " +  groupId);
    }

    public async Task OnGroupMemberJoined(Guid groupId, Guid memberId)
    {
        Console.WriteLine("ON GROUP MEMBER JOINED "  + groupId + " " +memberId);
    }

    public async Task OnGroupMemberLeft(Guid groupId, Guid memberId)
    {
        Console.WriteLine("ON GROUP MEMBER LEFTs "  + groupId + " " +memberId);
    }
}