using Dara.Server.Modules.Identity.Domain.Users;

namespace Dara.Server.Modules.Identity.UnitTests;

// a readable stand-in for the slow hash; "old:" marks a hash made with older settings
public class FakePasswordHasher : IPasswordHasher
{
    public string Hash(string password) => "hash:" + password;

    public PasswordCheck Verify(string passwordHash, string password)
    {
        if (passwordHash == "hash:" + password)
            return PasswordCheck.Success;

        return passwordHash == "old:" + password ? PasswordCheck.SuccessRehashNeeded : PasswordCheck.Failed;
    }
}

public class FakeUniquenessChecker : IUserUniquenessChecker
{
    public HashSet<string> Logins { get; } = new();
    public HashSet<string> Emails { get; } = new();

    public bool IsLoginTaken(string normalizedLogin) => Logins.Contains(normalizedLogin);

    public bool IsEmailTaken(string normalizedEmail) => Emails.Contains(normalizedEmail);
}
