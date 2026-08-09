using Dara.Server.Apps.API.Hubs;
using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Groups.Application;
using Dara.Server.Modules.Groups.Application.GetGroupDetails;
using Dara.Server.Modules.Groups.Integration;

namespace Dara.Server.Apps.API.Events;

public class NewMemberJoinedGroupEventHandler : IIntegrationEventHandler<NewMemberJoinedGroupIntegrationEvent>
{
    private readonly IServiceProvider _serviceProvider;

    public NewMemberJoinedGroupEventHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task HandleAsync(NewMemberJoinedGroupIntegrationEvent integrationEvent)
    {
        using var scope = _serviceProvider.CreateScope();
        var client = AppHub.GetClientByGuid(integrationEvent.MemberId);
        var groups = scope.ServiceProvider.GetRequiredService<IGroupModule>();
        var group = await groups.ExecuteQueryAsync<GetGroupDetailsQuery, GroupDetailsDto>(new(integrationEvent.GroupId));

        await client.OnGroupJoined(group.GroupId,group.GroupName,group.JoinCode,group.Members);
        foreach (var member in group.Members)
        {
            await AppHub.GetClientByGuid(member).OnGroupMemberJoined(group.GroupId, member);
        }
    }
}