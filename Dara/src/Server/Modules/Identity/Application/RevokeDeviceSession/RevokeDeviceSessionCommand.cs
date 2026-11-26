using System.Text.Json.Serialization;
using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Identity.Application.RevokeDeviceSession;

// queued by the handler of the revocation event of the devices module (TASK-ENG-029)
public record RevokeDeviceSessionCommand(Guid DeviceId) : IOrderedInternalCommand
{
    // queued commands of one device run in order
    [JsonIgnore]
    public string OrderingKey => $"device:{DeviceId}";
}
