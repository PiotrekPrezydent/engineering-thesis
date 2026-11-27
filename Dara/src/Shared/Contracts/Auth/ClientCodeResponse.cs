namespace Dara.Shared.Contracts.Auth;

// a registration code or a login code, shown once by the panel and typed in the app of the client
public record ClientCodeResponse(string Code, DateTime ExpiresAt);
