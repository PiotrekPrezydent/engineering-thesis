namespace Dara.Server.BuildingBlocks.Application.Events;

public record OutboxData(Guid MessageId, DateTime OccuredOn);