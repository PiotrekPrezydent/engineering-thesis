using System.Text.Json.Serialization;
using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Authorization.Application.RoleAssignments.UnassignRole;

// a module took a role on one of its resources from a subject (RevokeScopeRoleIntegrationEvent)
public record UnassignRoleCommand(Guid SubjectId, string RoleCode, Guid ScopeId) : IOrderedInternalCommand
{
    [JsonIgnore]
    public string OrderingKey => $"scope:{ScopeId}";
}
