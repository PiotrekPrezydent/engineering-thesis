using System.Text.Json.Serialization;
using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Plugins.Application.PluginOwners.CreatePluginOwner;

public record CreatePluginOwnerCommand(Guid OwnerId) : IOrderedInternalCommand
{
    // queued commands of one owner run in order
    [JsonIgnore]
    public string OrderingKey => $"owner:{OwnerId}";
}
