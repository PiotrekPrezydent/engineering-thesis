namespace Dara.Server.Modules.Identity.Domain.Users;

public enum PasswordCheck
{
    Failed,
    Success,
    // the password is right, but the hash was made with older settings and is replaced
    SuccessRehashNeeded
}
