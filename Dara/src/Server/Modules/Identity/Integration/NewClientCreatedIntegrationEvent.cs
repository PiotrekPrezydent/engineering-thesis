using Dara.Server.BuildingBlocks.Integration;

namespace Dara.Server.Modules.Identity.Integration;

public record NewClientCreatedIntegrationEvent(Guid CreatedClientId) : IntegrationEventBase;