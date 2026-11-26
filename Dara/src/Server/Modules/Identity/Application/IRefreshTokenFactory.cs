namespace Dara.Server.Modules.Identity.Application;

// a refresh token has 256 random bits, so a fast hash is enough to keep it out of the database
public interface IRefreshTokenFactory
{
    NewRefreshToken Create();

    string Hash(string refreshToken);
}

// the token goes to the client once, only the hash is stored
public record NewRefreshToken(string Token, string Hash);
