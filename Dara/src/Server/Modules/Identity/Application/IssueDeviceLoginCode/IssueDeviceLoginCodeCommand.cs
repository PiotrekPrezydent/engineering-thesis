using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Identity.Application.IssueDeviceLoginCode;

// the account panel asks for a code for one of its devices; the host takes UserId from the account session
public record IssueDeviceLoginCodeCommand(Guid UserId, Guid DeviceId) : ICommand<DeviceLoginCodeResult>;
