using Dara.Server.BuildingBlocks.Application.Authorization;

namespace Dara.Server.Modules.Orchestration.Application;

// what a subject may do on one resource of a scope of the module, the code starts with the path of the scope; the
// endpoints of the module are marked with them once the owner decides who may do what (TASK-ENG-040-D08)
public static class OrchestrationPermissions
{
    public static readonly PermissionDefinition ReadAgentRun = new("orchestration.group.read_agent_run", OrchestrationScopes.Group);
    public static readonly PermissionDefinition EditAgentSettings = new("orchestration.group.edit_agent_settings", OrchestrationScopes.Group);
    public static readonly PermissionDefinition ApproveExecutionPlan = new("orchestration.group.approve_execution_plan", OrchestrationScopes.Group);
    public static readonly PermissionDefinition ManageAccess = new(OrchestrationScopes.Group.ManageAccessPermissionCode, OrchestrationScopes.Group);
}
