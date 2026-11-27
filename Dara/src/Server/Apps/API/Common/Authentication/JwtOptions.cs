using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Dara.Server.Apps.API.Common.Authentication;

// configuration section "Jwt"; the signing key lives only in .env (Jwt__SigningKey), never in the repository
public class JwtOptions
{
    public const string SectionName = "Jwt";
    public const int MinSigningKeyBytes = 32;

    public string SigningKey { get; init; } = "";
    public string Issuer { get; init; } = "dara";
    public string Audience { get; init; } = "dara-clients";
    // client tokens only, the account panel has a cookie
    public TimeSpan AccessTokenLifetime { get; init; } = TimeSpan.FromMinutes(15);

    public static JwtOptions FromConfiguration(IConfiguration configuration)
    {
        var options = configuration.GetSection(SectionName).Get<JwtOptions>() ?? new JwtOptions();

        if (Encoding.UTF8.GetByteCount(options.SigningKey) < MinSigningKeyBytes)
            throw new InvalidOperationException(
                $"Jwt__SigningKey is missing or shorter than {MinSigningKeyBytes} bytes: set it in .env next to the api (see .env.example)");

        if (options.AccessTokenLifetime <= TimeSpan.Zero)
            throw new InvalidOperationException("Jwt__AccessTokenLifetime must be positive");

        return options;
    }

    public SymmetricSecurityKey CreateSigningKey()
    {
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SigningKey));
    }
}
