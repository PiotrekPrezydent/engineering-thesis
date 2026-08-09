using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Groups.Domain.GroupMessages;

public record GroupMessageId(Guid Value) : BaseEntityId(Value);