using Dara.Server.BuildingBlocks.Application.Authorization;

namespace Dara.Server.Modules.Orchestration.Application;

// the kinds of resources of the module whose access is controlled: the agent of one group (ScopeId = the group id): its settings, runs and execution plans
public static class OrchestrationScopes
{
    public static readonly ScopeDefinition Group = new("Orchestration.Group");
}
