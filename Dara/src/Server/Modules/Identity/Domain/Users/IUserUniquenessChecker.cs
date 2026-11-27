namespace Dara.Server.Modules.Identity.Domain.Users;

// the arguments are already normalized (login lower case, e-mail upper case)
public interface IUserUniquenessChecker
{
    bool IsLoginTaken(string normalizedLogin);

    bool IsEmailTaken(string normalizedEmail);
}
