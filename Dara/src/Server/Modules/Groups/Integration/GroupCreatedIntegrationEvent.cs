using Dara.Server.BuildingBlocks.Integration;

namespace Dara.Server.Modules.Groups.Integration;

public record GroupCreatedIntegrationEvent(Guid GroupId, Guid GroupOwnerId) : IntegrationEventBase;