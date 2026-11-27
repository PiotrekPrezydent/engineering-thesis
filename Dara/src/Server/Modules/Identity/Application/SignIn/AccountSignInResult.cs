namespace Dara.Server.Modules.Identity.Application.SignIn;

// the account, its panel client (the subject of the cookie in Authorization) and the sign-in of this browser
public record AccountSignInResult(AuthError? Error, Guid UserId, Guid PanelClientId, Guid PanelSignInId)
{
    public bool Succeeded => Error == null;

    public static AccountSignInResult Failed(AuthError error) => new(error, Guid.Empty, Guid.Empty, Guid.Empty);

    public static AccountSignInResult SignedIn(Guid userId, Guid panelClientId, Guid panelSignInId) => new(null, userId, panelClientId, panelSignInId);
}
