using System.Net.Mail;
using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Identity.Domain.Users.Rules;

public class EmailMustHaveValidFormatRule : IBuisnessRule
{
    public const int MaxLength = 254;

    private readonly string _email;

    public EmailMustHaveValidFormatRule(string email)
    {
        _email = email;
    }

    public string Message => $"E-mail must be a valid address of at most {MaxLength} characters";

    public bool IsBroken()
    {
        if (string.IsNullOrWhiteSpace(_email) || _email.Length > MaxLength)
            return true;

        // a display name ("Name <a@b>") parses too, so the parsed address must be the whole input
        return !MailAddress.TryCreate(_email, out var address) || address.Address != _email;
    }
}
