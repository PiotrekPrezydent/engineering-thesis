namespace Dara.Server.Modules.Identity.Application;

// the result of a registration, a sign-in and a refresh of a client; a refusal is a result, not an exception, so the
// host maps it to a status code; the raw refresh token is only here
public record ClientTokensResult(
    AuthError? Error,
    Guid UserId,
    Guid ClientId,
    string? RefreshToken,
    DateTime? RefreshTokenExpiresAt)
{
    public bool Succeeded => Error == null;

    public static ClientTokensResult Failed(AuthError error) =>
        new(error, Guid.Empty, Guid.Empty, null, null);

    public static ClientTokensResult Issued(Guid userId, Guid clientId, string refreshToken, DateTime refreshTokenExpiresAt) =>
        new(null, userId, clientId, refreshToken, refreshTokenExpiresAt);
}
