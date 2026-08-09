using Dara.Server.BuildingBlocks.Integration;

namespace Dara.Server.Modules.Profiles.Integration;

public record ProfileNameChangedIntegrationEvent(Guid ProfileId, string NewName) : IntegrationEventBase;