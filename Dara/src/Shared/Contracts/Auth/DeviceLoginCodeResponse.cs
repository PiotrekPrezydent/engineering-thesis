namespace Dara.Shared.Contracts.Auth;

// shown once by the panel and typed on the device; a new code replaces the previous one
public record DeviceLoginCodeResponse(string Code, DateTime ExpiresAt);
