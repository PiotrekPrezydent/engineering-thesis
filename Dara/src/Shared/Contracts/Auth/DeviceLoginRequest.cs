namespace Dara.Shared.Contracts.Auth;

// the one-time code the account panel showed for this device
public record DeviceLoginRequest(string Code);
