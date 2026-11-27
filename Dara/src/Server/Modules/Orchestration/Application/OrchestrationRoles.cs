using Dara.Server.BuildingBlocks.Application.Authorization;

namespace Dara.Server.Modules.Orchestration.Application;

public static class OrchestrationRoles
{
    private const string OwnerCode = "Orchestration.Owner";

    // follows the runs
    public static readonly RoleDefinition Viewer = new("Orchestration.Viewer", OrchestrationScopes.Group, [OrchestrationPermissions.ReadAgentRun]);

    // approves or rejects execution plans
    public static readonly RoleDefinition Operator = new("Orchestration.Operator", OrchestrationScopes.Group, [OrchestrationPermissions.ApproveExecutionPlan])
    {
        Subordinates = [Viewer]
    };

    // sets up the agent of the group and manages the access
    public static readonly RoleDefinition Owner = new(OwnerCode, OrchestrationScopes.Group, [OrchestrationPermissions.EditAgentSettings, OrchestrationPermissions.ManageAccess])
    {
        Subordinates = [Operator],
        ManageableRoleCodes = [OwnerCode, Operator.Code, Viewer.Code]
    };
}
