using Dara.Server.BuildingBlocks.Application.Authorization;

namespace Dara.Server.Modules.Orchestration.Application;

// the authorization definition of the module, checked at start-up and synchronized into Authorization
internal sealed class OrchestrationAuthorizationDefinition : IModuleAuthorizationDefinition
{
    public string ModuleName => "Orchestration";

    public IEnumerable<ScopeDefinition> GetScopes() => [OrchestrationScopes.Group];

    public IEnumerable<PermissionDefinition> GetPermissions() => [OrchestrationPermissions.ReadAgentRun, OrchestrationPermissions.EditAgentSettings, OrchestrationPermissions.ApproveExecutionPlan, OrchestrationPermissions.ManageAccess];

    public IEnumerable<RoleDefinition> GetDefaultRoles() => [OrchestrationRoles.Owner, OrchestrationRoles.Operator, OrchestrationRoles.Viewer];
}
