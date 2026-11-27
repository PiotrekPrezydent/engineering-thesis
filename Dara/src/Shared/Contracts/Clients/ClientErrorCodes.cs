using Dara.Shared.Contracts.Auth;

namespace Dara.Shared.Contracts.Clients;

// the "code" field of the problem details of a refused request of /account/clients/* and /clients/current; a broken
// rule (400) also names the rule in the "rule" field
public static class ClientErrorCodes
{
    // 400
    public const string RuleBroken = AuthErrorCodes.RuleBroken;
    // 404, a missing client and a client of another account look the same
    public const string ClientNotFound = AuthErrorCodes.ClientNotFound;
    // 409, a revoked client takes no change
    public const string ClientRevoked = AuthErrorCodes.ClientRevoked;
}
