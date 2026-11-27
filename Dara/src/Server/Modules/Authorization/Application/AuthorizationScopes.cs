using Dara.Server.BuildingBlocks.Application.Authorization;

namespace Dara.Server.Modules.Authorization.Application;

// the server as a whole, only with ScopeIds.All
public static class AuthorizationScopes
{
    public static readonly ScopeDefinition Server = new("Authorization.Server");
}
