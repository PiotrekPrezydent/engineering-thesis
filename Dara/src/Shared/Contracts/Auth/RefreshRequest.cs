namespace Dara.Shared.Contracts.Auth;

// a refresh token is used once: the answer has a new one
public record RefreshRequest(string RefreshToken);
