namespace Dara.Shared.Contracts.Account;

// the token goes into the form field or the header of these names, next to the antiforgery cookie the answer sets
public record AntiforgeryTokenResponse(string RequestToken, string FormFieldName, string? HeaderName);
