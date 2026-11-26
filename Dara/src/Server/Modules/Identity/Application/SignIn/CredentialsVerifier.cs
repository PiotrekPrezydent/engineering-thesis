using Dara.Server.Modules.Identity.Domain;
using Dara.Server.Modules.Identity.Domain.Rules;

namespace Dara.Server.Modules.Identity.Application.SignIn;

// the password check of the panel sign-in; an unknown login still costs one slow hash,
// so the response time does not tell whether the account exists
public class CredentialsVerifier
{
    private static readonly object HashLock = new();
    private static string? _unknownUserHash;

    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public CredentialsVerifier(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    // null when the login or the password is wrong
    public async Task<User?> VerifyAsync(string? login, string? password)
    {
        var user = await _userRepository.FindByLoginAsync(login ?? string.Empty);
        var tooLong = password is { Length: > PasswordMustMeetPolicyRule.MaxLength };

        if (user == null || password == null || tooLong)
        {
            _passwordHasher.Verify(UnknownUserHash(), tooLong ? string.Empty : password ?? string.Empty);
            return null;
        }

        return user.VerifyPassword(password, _passwordHasher) == PasswordCheck.Failed ? null : user;
    }

    private string UnknownUserHash()
    {
        lock (HashLock)
        {
            return _unknownUserHash ??= _passwordHasher.Hash(Guid.NewGuid().ToString("N"));
        }
    }
}
