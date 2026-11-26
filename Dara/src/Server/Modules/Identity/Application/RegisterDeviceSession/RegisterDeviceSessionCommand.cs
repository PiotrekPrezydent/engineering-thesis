using System.Text.Json.Serialization;
using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Identity.Application.RegisterDeviceSession;

// identity keeps its own copy of the devices of an account, so it never asks the devices module: the seed and tests
// call it today (one device per account, DeviceId = userId), the handler of the device creation event of the devices
// module queues it later (TASK-ENG-029)
public record RegisterDeviceSessionCommand(Guid DeviceId, Guid UserId) : IOrderedInternalCommand
{
    // queued commands of one device run in order
    [JsonIgnore]
    public string OrderingKey => $"device:{DeviceId}";
}
