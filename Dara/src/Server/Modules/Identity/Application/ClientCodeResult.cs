namespace Dara.Server.Modules.Identity.Application;

// a registration code or a login code of the panel; the raw code is only here, the panel shows it once
public record ClientCodeResult(AuthError? Error, string? Code, DateTime? ExpiresAt)
{
    public bool Succeeded => Error == null;

    public static ClientCodeResult Failed(AuthError error) => new(error, null, null);

    public static ClientCodeResult Issued(string code, DateTime expiresAt) => new(null, code, expiresAt);
}
