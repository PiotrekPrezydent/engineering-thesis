using Dara.Server.BuildingBlocks.Domain.Roles;

namespace Dara.Server.Modules.Orchestration.Domain;

public sealed record OrchestrationRole(OrchestrationRole.Roles Value) : EntityRole<OrchestrationRole.Roles>(Value)
{
    public enum Roles
    {
        Owner,
        Viewer
    }
}
