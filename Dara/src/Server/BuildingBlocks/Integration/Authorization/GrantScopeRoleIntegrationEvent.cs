namespace Dara.Server.BuildingBlocks.Integration.Authorization;

// a module gives a subject (any id: a user, a client, a group) a role on one of its resources, or on all of them with
// ScopeIds.All, after its own command or an event of another module; Authorization subscribes, the module needs no
// reference to it
public record GrantScopeRoleIntegrationEvent : IntegrationEventBase
{
    public Guid SubjectId { get; }
    public string RoleCode { get; }
    public string ScopeCode { get; }
    public Guid ScopeId { get; }

    public GrantScopeRoleIntegrationEvent(Guid eventId, DateTime occurredOn, Guid subjectId, string roleCode, string scopeCode,
        Guid scopeId) : base(eventId, occurredOn)
    {
        SubjectId = subjectId;
        RoleCode = roleCode;
        ScopeCode = scopeCode;
        ScopeId = scopeId;
    }
}
