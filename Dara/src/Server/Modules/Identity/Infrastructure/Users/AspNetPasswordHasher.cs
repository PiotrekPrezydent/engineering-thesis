using Dara.Server.Modules.Identity.Domain;
using Microsoft.AspNetCore.Identity;

namespace Dara.Server.Modules.Identity.Infrastructure.Users;

// PasswordHasher<T> with default options: PBKDF2-HMAC-SHA512 with a random salt, the settings are kept in the hash,
// so a later change of settings is recognized and the hash is replaced on the next sign-in
public class AspNetPasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<User> _hasher = new();

    // the default implementation does not use the user, the hash carries everything it needs
    public string Hash(string password)
    {
        return _hasher.HashPassword(null!, password);
    }

    public PasswordCheck Verify(string passwordHash, string password)
    {
        return _hasher.VerifyHashedPassword(null!, passwordHash, password) switch
        {
            PasswordVerificationResult.Success => PasswordCheck.Success,
            PasswordVerificationResult.SuccessRehashNeeded => PasswordCheck.SuccessRehashNeeded,
            _ => PasswordCheck.Failed
        };
    }
}
