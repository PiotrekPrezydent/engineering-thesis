namespace Dara.Server.Modules.Identity.Application.SignIn;

public record AccountSignInResult(AuthError? Error, Guid UserId, Guid AccountSessionId)
{
    public bool Succeeded => Error == null;

    public static AccountSignInResult Failed(AuthError error) => new(error, Guid.Empty, Guid.Empty);

    public static AccountSignInResult SignedIn(Guid userId, Guid accountSessionId) => new(null, userId, accountSessionId);
}
