namespace Dara.Server.BuildingBlocks.Application.Authorization;

// exactly one implementation in the Application assembly of every module; gives the roles of the module
// and the permissions of the module each role allows, the module checks them at start-up
public interface IModuleRoles
{
    void Configure(ModuleRolesBuilder roles);
}
