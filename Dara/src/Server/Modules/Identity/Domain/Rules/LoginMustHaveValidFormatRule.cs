using System.Text.RegularExpressions;
using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Identity.Domain.Rules;

public partial class LoginMustHaveValidFormatRule : IBuisnessRule
{
    private readonly string _normalizedLogin;

    public LoginMustHaveValidFormatRule(string normalizedLogin)
    {
        _normalizedLogin = normalizedLogin;
    }

    public string Message => "Login must have 3 to 32 characters: lower case letters, digits, '.', '_' or '-'";

    public bool IsBroken()
    {
        return !LoginPattern().IsMatch(_normalizedLogin);
    }

    [GeneratedRegex("^[a-z0-9._-]{3,32}$")]
    private static partial Regex LoginPattern();
}
