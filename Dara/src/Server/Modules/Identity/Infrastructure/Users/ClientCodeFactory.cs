using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;
using Dara.Server.Modules.Identity.Application;

namespace Dara.Server.Modules.Identity.Infrastructure.Users;

// 8 characters without the ones easy to mix up (0 and O, 1, I and L), about 40 bits; a code lives 10 minutes, is
// used once and the endpoints that take it have a rate limit, so SHA-256 is enough to keep it out of the database
public class ClientCodeFactory : IClientCodeFactory
{
    public const string Alphabet = "23456789ABCDEFGHJKMNPQRSTUVWXYZ";
    public const int Length = 8;

    public NewClientCode Create()
    {
        var code = RandomNumberGenerator.GetString(Alphabet, Length);
        return new NewClientCode(code, Hash(code));
    }

    public string Hash(string code)
    {
        return Base64Url.EncodeToString(SHA256.HashData(Encoding.UTF8.GetBytes(Normalize(code))));
    }

    // a person may type small letters or split the code with spaces or dashes
    private static string Normalize(string code)
    {
        return new string(code.Where(c => !char.IsWhiteSpace(c) && c != '-').Select(char.ToUpperInvariant).ToArray());
    }
}
