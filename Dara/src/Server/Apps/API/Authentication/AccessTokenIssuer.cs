using Dara.Shared.Contracts.Auth;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Dara.Server.Apps.API.Authentication;

// the host, not the identity module, issues access tokens: the token is a matter of transport, one key in one place
public class AccessTokenIssuer
{
    private readonly JwtOptions _options;
    private readonly SigningCredentials _credentials;
    private readonly TimeProvider _timeProvider;
    private readonly JsonWebTokenHandler _handler = new();

    public AccessTokenIssuer(JwtOptions options, TimeProvider timeProvider)
    {
        _options = options;
        _timeProvider = timeProvider;
        _credentials = new SigningCredentials(options.CreateSigningKey(), SecurityAlgorithms.HmacSha256);
    }

    // only devices get access tokens; the account panel keeps its session in a cookie
    public AccessToken IssueDevice(Guid userId, Guid deviceId)
    {
        var now = _timeProvider.GetUtcNow().UtcDateTime;
        var expiresAt = now + _options.AccessTokenLifetime;

        var claims = new Dictionary<string, object>
        {
            [AuthClaims.Subject] = userId.ToString(),
            [AuthClaims.Scope] = AuthClaims.ScopeDevice,
            [AuthClaims.Device] = deviceId.ToString(),
            [JwtRegisteredClaimNames.Jti] = Guid.NewGuid().ToString()
        };

        var token = _handler.CreateToken(new SecurityTokenDescriptor
        {
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            IssuedAt = now,
            NotBefore = now,
            Expires = expiresAt,
            Claims = claims,
            SigningCredentials = _credentials
        });

        return new AccessToken(token, expiresAt);
    }
}

public record AccessToken(string Token, DateTime ExpiresAt);
