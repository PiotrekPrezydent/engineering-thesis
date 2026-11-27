using Dara.Server.BuildingBlocks.Application.Authorization;

namespace Dara.Server.Modules.Identity.Application;

// the kinds of resources of the module whose access is controlled: the account (ScopeId = the user id: its clients and their codes) and one client (ScopeId = the client id)
public static class IdentityScopes
{
    public static readonly ScopeDefinition User = new("Identity.User");
    public static readonly ScopeDefinition Client = new("Identity.Client");
}
