namespace Dara.Shared.Contracts.Auth;

// the tokens of a device after device-login and after every refresh; the client keeps DeviceId from here
public record AuthTokensResponse(
    string AccessToken,
    DateTime AccessTokenExpiresAt,
    string RefreshToken,
    DateTime RefreshTokenExpiresAt,
    Guid UserId,
    Guid DeviceId);
