namespace Dara.Server.Modules.Identity.Application;

// the result of a device sign-in and of a refresh; a refusal is a result, not an exception, so the host maps it
// to a status code; the raw refresh token is only here
public record DeviceTokensResult(
    AuthError? Error,
    Guid UserId,
    Guid DeviceId,
    string? RefreshToken,
    DateTime? RefreshTokenExpiresAt)
{
    public bool Succeeded => Error == null;

    public static DeviceTokensResult Failed(AuthError error) =>
        new(error, Guid.Empty, Guid.Empty, null, null);

    public static DeviceTokensResult Issued(Guid userId, Guid deviceId, string refreshToken, DateTime refreshTokenExpiresAt) =>
        new(null, userId, deviceId, refreshToken, refreshTokenExpiresAt);
}
