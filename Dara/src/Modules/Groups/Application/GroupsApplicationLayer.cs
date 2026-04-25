using Dara.BuildingBlocks.Application;
using Dara.BuildingBlocks.Application.Integration;

namespace Dara.Modules.Groups.Application;

public class GroupsApplicationLayer : IApplicationLayer
{
    public IReadOnlyList<Type> GetModuleCommandHandlers => [];
    public IReadOnlyList<Type> GetDomainEventHandlers => [];
    public IReadOnlyList<IIntegrationEventHandler> GetIntegrationEventHandlers => 
    [
        new ConnectionCreatedHandler()
    ];
}