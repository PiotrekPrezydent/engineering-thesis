using Dara.Server.BuildingBlocks.Domain.Roles;

namespace Dara.Server.Modules.Identity.Domain;

public sealed record IdentityRole(IdentityRole.Roles Value) : EntityRole<IdentityRole.Roles>(Value)
{
    public enum Roles
    {
        Owner,
        Viewer
    }
}
