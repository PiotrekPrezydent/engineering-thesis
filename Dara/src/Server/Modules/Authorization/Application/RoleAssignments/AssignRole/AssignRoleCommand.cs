using System.Text.Json.Serialization;
using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Authorization.Application.RoleAssignments.AssignRole;

// a module gave a subject a role on one of its resources or on all of them (GrantScopeRoleIntegrationEvent)
public record AssignRoleCommand(Guid SubjectId, string RoleCode, string ScopeCode, Guid ScopeId) : IOrderedInternalCommand
{
    // the changes of one resource run in the order the module made them
    [JsonIgnore]
    public string OrderingKey => $"scope:{ScopeId}";
}
