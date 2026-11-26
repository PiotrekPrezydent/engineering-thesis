namespace Dara.Shared.Contracts.Auth;

// the session itself is in the HttpOnly cookie of the answer, the panel never sees it
public record AccountSignInResponse(Guid UserId);
