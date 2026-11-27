namespace Dara.Server.BuildingBlocks.Integration.Authorization;

// a module takes a role on one of its resources (or a global one) from a subject after its own command
public record RevokeScopeRoleIntegrationEvent : IntegrationEventBase
{
    public Guid SubjectId { get; }
    public string RoleCode { get; }
    public string ScopeCode { get; }
    public Guid ScopeId { get; }

    public RevokeScopeRoleIntegrationEvent(Guid eventId, DateTime occurredOn, Guid subjectId, string roleCode, string scopeCode,
        Guid scopeId) : base(eventId, occurredOn)
    {
        SubjectId = subjectId;
        RoleCode = roleCode;
        ScopeCode = scopeCode;
        ScopeId = scopeId;
    }
}
