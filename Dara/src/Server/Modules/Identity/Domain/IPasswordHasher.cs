namespace Dara.Server.Modules.Identity.Domain;

// a slow salted hash of a password; the implementation lives in the infrastructure
public interface IPasswordHasher
{
    string Hash(string password);

    PasswordCheck Verify(string passwordHash, string password);
}
