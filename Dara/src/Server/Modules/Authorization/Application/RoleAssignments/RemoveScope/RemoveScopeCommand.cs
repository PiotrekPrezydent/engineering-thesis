using System.Text.Json.Serialization;
using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Authorization.Application.RoleAssignments.RemoveScope;

// a resource is gone for good (ScopeRemovedIntegrationEvent)
public record RemoveScopeCommand(string ScopeCode, Guid ScopeId) : IOrderedInternalCommand
{
    [JsonIgnore]
    public string OrderingKey => $"scope:{ScopeId}";
}
