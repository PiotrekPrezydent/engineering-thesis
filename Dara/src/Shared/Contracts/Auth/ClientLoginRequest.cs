namespace Dara.Shared.Contracts.Auth;

// the one-time login code the account panel showed for this client
public record ClientLoginRequest(string Code);
