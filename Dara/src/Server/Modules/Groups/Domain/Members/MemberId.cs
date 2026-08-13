using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Groups.Domain.Members;

public record MemberId(Guid Value) : BaseEntityId(Value);