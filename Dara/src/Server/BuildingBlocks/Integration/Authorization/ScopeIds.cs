namespace Dara.Server.BuildingBlocks.Integration.Authorization;

public static class ScopeIds
{
    // "*": the ScopeId of an assignment on every resource of its scope (a global role); granted only automatically,
    // by the events of modules and the seed, never through the api
    public static readonly Guid All = Guid.Empty;
}
