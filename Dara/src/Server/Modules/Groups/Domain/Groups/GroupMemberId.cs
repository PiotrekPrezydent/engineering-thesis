using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Groups.Domain.Groups;

public record GroupMemberId : BaseEntityId
{
    public GroupMemberId(Guid value) : base(value)
    {
    }
}