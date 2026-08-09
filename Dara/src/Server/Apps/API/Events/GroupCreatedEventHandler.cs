using Dara.Server.Apps.API.Hubs;
using Dara.Server.BuildingBlocks.Application.Events;
using Dara.Server.Modules.Groups.Application;
using Dara.Server.Modules.Groups.Application.GetGroupDetails;
using Dara.Server.Modules.Groups.Integration;

namespace Dara.Server.Apps.API.Events;

public class GroupCreatedEventHandler : IIntegrationEventHandler<GroupCreatedIntegrationEvent>
{
    private readonly IServiceProvider _serviceProvider;
    
    public GroupCreatedEventHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public async Task HandleAsync(GroupCreatedIntegrationEvent integrationEvent)
    {
        using var scope = _serviceProvider.CreateScope();
        var owner = AppHub.GetClientByGuid(integrationEvent.GroupOwnerId);
        var groupsModule = scope.ServiceProvider.GetRequiredService<IGroupModule>();

        var group = await groupsModule.ExecuteQueryAsync<GetGroupDetailsQuery, GroupDetailsDto>(
            new GetGroupDetailsQuery(integrationEvent.GroupId)); 
        
        await owner.OnGroupCreated(group.GroupId,group.GroupName,group.JoinCode,group.Members);
    }
}