namespace Dara.Server.BuildingBlocks.Application.Authorization;

// what a user may do in a module; a type of its own, commands and queries know nothing about it,
// the code is the name of the implementing type and the permission, e.g. "GroupsPermission.SendMessage"
public interface IModulePermission
{
    string PermissionCode { get; }
}
