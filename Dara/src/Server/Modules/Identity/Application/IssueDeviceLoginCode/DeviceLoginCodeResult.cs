namespace Dara.Server.Modules.Identity.Application.IssueDeviceLoginCode;

// the raw code is only here, the panel shows it once
public record DeviceLoginCodeResult(AuthError? Error, string? Code, DateTime? ExpiresAt)
{
    public bool Succeeded => Error == null;

    public static DeviceLoginCodeResult Failed(AuthError error) => new(error, null, null);

    public static DeviceLoginCodeResult Issued(string code, DateTime expiresAt) => new(null, code, expiresAt);
}
