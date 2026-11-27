using Dara.Server.BuildingBlocks.Application.Authorization;

namespace Dara.Server.Modules.Groups.Application;

// what a user may do in the Groups module; commands and queries do not use it, endpoints of the API are marked with it
public sealed record GroupsPermission(GroupsPermission.Permissions Value) : ModulePermission<GroupsPermission.Permissions>(Value)
{
    public enum Permissions
    {
        ReadGroups,
        CreateGroup,
        JoinGroup,
        LeaveGroup,
        SendMessage
    }
}
