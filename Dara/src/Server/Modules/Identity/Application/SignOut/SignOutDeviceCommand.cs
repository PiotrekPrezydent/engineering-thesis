using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Identity.Application.SignOut;

// the host takes both ids from the claims of the device token
public record SignOutDeviceCommand(Guid UserId, Guid DeviceId) : ICommand;
