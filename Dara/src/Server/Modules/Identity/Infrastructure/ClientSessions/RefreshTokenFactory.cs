using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;
using Dara.Server.Modules.Identity.Application;

namespace Dara.Server.Modules.Identity.Infrastructure.ClientSessions;

// 32 random bytes in base64url; the stored hash is SHA-256, a slow hash adds nothing for 256 random bits
public class RefreshTokenFactory : IRefreshTokenFactory
{
    private const int TokenBytes = 32;

    public NewRefreshToken Create()
    {
        var token = Base64Url.EncodeToString(RandomNumberGenerator.GetBytes(TokenBytes));
        return new NewRefreshToken(token, Hash(token));
    }

    public string Hash(string refreshToken)
    {
        return Base64Url.EncodeToString(SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken)));
    }
}
