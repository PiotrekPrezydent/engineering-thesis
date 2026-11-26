using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Identity.Application.SignIn;

// a device signs in only with the one-time code the account panel issued for it, never with the password
public record SignInDeviceCommand(string Code) : ICommand<DeviceTokensResult>;
