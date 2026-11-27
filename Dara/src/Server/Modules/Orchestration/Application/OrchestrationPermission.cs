using Dara.Server.BuildingBlocks.Application.Authorization;

namespace Dara.Server.Modules.Orchestration.Application;

// what a user may do in the Orchestration module; commands and queries do not use it, endpoints of the API are marked with it
public sealed record OrchestrationPermission(OrchestrationPermission.Permissions Value) : ModulePermission<OrchestrationPermission.Permissions>(Value)
{
    public enum Permissions
    {
        ReadAgentSettings,
        EditAgentSettings,
        ReadAgentRun,
        ApproveExecutionPlan,
        RejectExecutionPlan
    }
}
