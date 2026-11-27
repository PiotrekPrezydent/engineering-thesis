using Dara.Server.BuildingBlocks.Domain.Roles;

namespace Dara.Server.Modules.Authorization.Domain;

public sealed record AuthorizationRole(AuthorizationRole.Roles Value) : EntityRole<AuthorizationRole.Roles>(Value)
{
    public enum Roles
    {
        Owner,
        Viewer
    }
}
