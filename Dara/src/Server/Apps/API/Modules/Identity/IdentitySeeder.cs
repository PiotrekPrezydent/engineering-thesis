using Dara.Server.Apps.API.Common.Seeding;
using Dara.Server.Modules.Identity.Application;
using Dara.Server.Modules.Identity.Application.ChangeClientContext;
using Dara.Server.Modules.Identity.Application.CreateClientRegistrationCode;
using Dara.Server.Modules.Identity.Application.RegisterClient;
using Dara.Server.Modules.Identity.Application.RegisterUser;

namespace Dara.Server.Apps.API.Modules.Identity;

// accounts with one shared password from .env (there is no registration endpoint yet) and their clients, registered
// like an app does it: the panel creates a registration code and the client registers itself with it
public class IdentitySeeder : ISeeder
{
    private readonly IIdentityModule _identityModule;
    private readonly SeedEnvironment _environment;
    private readonly ILogger<IdentitySeeder> _logger;

    public IdentitySeeder(IIdentityModule identityModule, SeedEnvironment environment, ILogger<IdentitySeeder> logger)
    {
        _identityModule = identityModule;
        _environment = environment;
        _logger = logger;
    }

    public async Task SeedAsync(SeedData data, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_environment.UserPassword))
            throw new InvalidOperationException("Seed__UserPassword is empty: set it in .env next to the api (see .env.example) or turn the seed off with Seed__Enabled=false");

        foreach (var account in data.Accounts)
        {
            var userId = await _identityModule.ExecuteCommandAsync<RegisterUserCommand, Guid>(
                new RegisterUserCommand(account.Login, account.Email, _environment.UserPassword));
            data.UserIds.Add(account.Login, userId);
        }

        foreach (var client in data.Clients)
            data.ClientIds.Add(client.Name, await RegisterClientAsync(data.UserIds[client.Account], client));

        await _identityModule.ExecuteCommandAsync(new ChangeClientContextCommand(
            data.UserIds["bob"], data.ClientIds["Bob-Laptop"],
            new Dictionary<string, string> { ["Folder zdjęć"] = @"C:\Users\bob\Pictures" }));

        _logger.LogInformation("Seeded {Accounts} accounts and {Clients} clients", data.UserIds.Count, data.ClientIds.Count);
    }

    private async Task<Guid> RegisterClientAsync(Guid userId, SeedClient client)
    {
        var code = await _identityModule.ExecuteCommandAsync<CreateClientRegistrationCodeCommand, ClientCodeResult>(
            new CreateClientRegistrationCodeCommand(userId));
        var tokens = await _identityModule.ExecuteCommandAsync<RegisterClientCommand, ClientTokensResult>(
            new RegisterClientCommand(code.Code!, client.ClientType, client.Name));

        if (!tokens.Succeeded)
            throw new InvalidOperationException($"seed could not register the client {client.Name}: {tokens.Error}");

        return tokens.ClientId;
    }
}
