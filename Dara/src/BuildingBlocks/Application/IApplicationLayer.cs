using Dara.BuildingBlocks.Application.Integration;

namespace Dara.BuildingBlocks.Application;

public interface IApplicationLayer
{
    public IReadOnlyList<Type> GetModuleCommandHandlers { get; }
    
    public IReadOnlyList<Type> GetDomainEventHandlers { get; }
    
    public IReadOnlyList<IIntegrationEventHandler> GetIntegrationEventHandlers { get; }
}