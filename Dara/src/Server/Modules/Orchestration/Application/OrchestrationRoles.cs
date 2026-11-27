using Dara.Server.BuildingBlocks.Application.Authorization;
using Dara.Server.Modules.Orchestration.Domain;

namespace Dara.Server.Modules.Orchestration.Application;

// Owner may do everything in the module, Viewer only read; both are given to every new account and client
internal sealed class OrchestrationRoles : IModuleRoles
{
    public void Configure(ModuleRolesBuilder roles) => roles
        .Role(new OrchestrationRole(OrchestrationRole.Roles.Owner), isDefault: true)
            .AllowsAll<OrchestrationPermission>()
        .Role(new OrchestrationRole(OrchestrationRole.Roles.Viewer), isDefault: true)
            .Allows(new OrchestrationPermission(OrchestrationPermission.Permissions.ReadAgentSettings), new OrchestrationPermission(OrchestrationPermission.Permissions.ReadAgentRun));
}
