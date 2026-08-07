using Dara.Server.BuildingBlocks.Integration;

namespace Dara.Server.Modules.Identity.Integration;

public record UserCreatedIntegrationEvent(Guid Id) : IIntegrationEvent;