using Dara.Server.BuildingBlocks.Application.Authorization;
using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Authorization.Application.Roles.SynchronizePermissionsAndRoles;

// the host sends it once at start-up with the checked catalogs of all modules; the only way permissions and roles change
public record SynchronizePermissionsAndRolesCommand(IReadOnlyList<ModuleAuthorizationCatalog> Catalogs) : ICommand;
