namespace Dara.Shared.Contracts.Auth;

// the registration code the account panel showed; Type is Pc, Phone, Tablet, Watch or Other (unknown text is Other),
// an empty name gets the label of the type with a number ("Phone 2")
public record RegisterClientRequest(string Code, string? Type, string? Name);
