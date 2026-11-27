using Dara.Server.BuildingBlocks.Domain.Roles;

namespace Dara.Server.Modules.Groups.Domain;

public sealed record GroupsRole(GroupsRole.Roles Value) : EntityRole<GroupsRole.Roles>(Value)
{
    public enum Roles
    {
        Owner,
        Viewer
    }
}
