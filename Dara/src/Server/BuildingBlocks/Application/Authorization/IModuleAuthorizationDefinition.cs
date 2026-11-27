namespace Dara.Server.BuildingBlocks.Application.Authorization;

// exactly one implementation in the Application assembly of every module, built from the static classes
// <Module>Scopes, <Module>Permissions and <Module>Roles; ModuleCompositionRootBase checks it at start-up and adds it
// to the services of the host, where Authorization reads the definitions of all modules after they are initialized;
// no module references Authorization
public interface IModuleAuthorizationDefinition
{
    string ModuleName { get; }
    IEnumerable<ScopeDefinition> GetScopes();
    IEnumerable<PermissionDefinition> GetPermissions();
    IEnumerable<RoleDefinition> GetDefaultRoles();

    // the roles the module itself grants every new user and every new client on the events of Identity; keep them few
    IEnumerable<NewSubjectRole> GetNewUserRoles() => [];
    IEnumerable<NewSubjectRole> GetNewClientRoles() => [];
}
