namespace Dara.Shared.Contracts.Auth;

// a sign-in of the account panel; a device never signs in with the password (DeviceLoginRequest)
public record LoginRequest(string Login, string Password);
