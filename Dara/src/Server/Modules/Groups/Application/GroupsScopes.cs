using Dara.Server.BuildingBlocks.Application.Authorization;

namespace Dara.Server.Modules.Groups.Application;

// the kinds of resources of the module whose access is controlled: one group (ScopeId = the group id)
public static class GroupsScopes
{
    public static readonly ScopeDefinition Group = new("Groups.Group");
}
