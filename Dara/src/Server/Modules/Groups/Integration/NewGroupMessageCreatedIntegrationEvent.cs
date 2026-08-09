using Dara.Server.BuildingBlocks.Integration;

namespace Dara.Server.Modules.Groups.Integration;

public record NewGroupMessageCreatedIntegrationEvent(Guid MessageId, Guid GroupId, Guid SenderId, string Content) : IntegrationEventBase;