using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Groups.Domain.Groups;

public record GroupMessageId : BaseEntityId
{
    public GroupMessageId(Guid value) : base(value)
    {
    }
}