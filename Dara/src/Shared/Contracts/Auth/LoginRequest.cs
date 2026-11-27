namespace Dara.Shared.Contracts.Auth;

// a sign-in of the account panel; a client never signs in with the password (RegisterClientRequest, ClientLoginRequest)
public record LoginRequest(string Login, string Password);
