using Dara.Server.Modules.Identity.Application;
using Dara.Server.Modules.Identity.Application.RegisterDeviceSession;
using Dara.Server.Modules.Identity.Application.RegisterUser;

namespace Dara.Server.Apps.API.Services.Seeding;

// accounts with one shared password from .env (there is no registration endpoint yet) and one device per account
// with the id of the account, so the account panel can issue a login code for it at once; TASK-ENG-029 replaces
// the device with the devices of its own seed
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

        foreach (var user in data.Users)
        {
            var userId = await _identityModule.ExecuteCommandAsync<RegisterUserCommand, Guid>(
                new RegisterUserCommand(user.Identifier, user.Email, _environment.UserPassword));
            await _identityModule.ExecuteCommandAsync(new RegisterDeviceSessionCommand(userId, userId));
            data.UserIds.Add(user.Identifier, userId);
        }

        _logger.LogInformation("Seeded {Count} users, each with one device of the same id", data.UserIds.Count);
    }
}
