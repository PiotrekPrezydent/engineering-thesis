namespace Dara.Server.Modules.Identity.Application;

// a short code a person types on the device; only its hash is stored
public interface IDeviceLoginCodeFactory
{
    NewDeviceLoginCode Create();

    // the same hash for the code typed with small letters, spaces or dashes
    string Hash(string code);
}

// the code goes to the panel once
public record NewDeviceLoginCode(string Code, string Hash);
