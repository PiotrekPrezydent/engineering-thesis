using Dara.Server.BuildingBlocks.Integration.Authorization;

namespace Dara.Server.BuildingBlocks.Application.Authorization;

// a role a module gives the panel client of every new account or every new client of the hub on the events of Identity: on every resource of its scope
// (a global role, ScopeIds.All, e.g. Plugins.Viewer to see the plugins of the server) or, OnItself, on the new
// subject itself (e.g. Identity.Owner on its own account); the module publishes the grant, not the server
public sealed record NewSubjectRole(RoleDefinition Role, bool OnItself = false)
{
    public GrantScopeRoleIntegrationEvent ToGrant(Guid subjectId, DateTime occurredOn)
    {
        return new GrantScopeRoleIntegrationEvent(Guid.NewGuid(), occurredOn, subjectId, Role.Code, Role.Scope.Code,
            OnItself ? subjectId : ScopeIds.All);
    }
}
