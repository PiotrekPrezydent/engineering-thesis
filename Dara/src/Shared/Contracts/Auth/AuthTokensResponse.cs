namespace Dara.Shared.Contracts.Auth;

// the tokens of a client after register-client, client-login and every refresh; the client keeps ClientId from here
public record AuthTokensResponse(
    string AccessToken,
    DateTime AccessTokenExpiresAt,
    string RefreshToken,
    DateTime RefreshTokenExpiresAt,
    Guid UserId,
    Guid ClientId);
