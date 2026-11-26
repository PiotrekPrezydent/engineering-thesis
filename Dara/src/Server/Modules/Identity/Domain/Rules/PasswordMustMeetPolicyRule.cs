using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Identity.Domain.Rules;

public class PasswordMustMeetPolicyRule : IBuisnessRule
{
    public const int MinLength = 8;
    // the upper limit keeps the slow hash away from very long input
    public const int MaxLength = 128;

    private readonly string? _password;

    public PasswordMustMeetPolicyRule(string? password)
    {
        _password = password;
    }

    public string Message => $"Password must have {MinLength} to {MaxLength} characters";

    public bool IsBroken()
    {
        return _password is null || _password.Length < MinLength || _password.Length > MaxLength;
    }
}
