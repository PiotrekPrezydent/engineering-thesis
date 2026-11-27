namespace Dara.Server.Modules.Identity.Application;

// a short code a person types in the app of a client (a registration code or a login code); only its hash is stored
public interface IClientCodeFactory
{
    NewClientCode Create();

    // the same hash for the code typed with small letters, spaces or dashes
    string Hash(string code);
}

// the code goes to the panel once
public record NewClientCode(string Code, string Hash);
