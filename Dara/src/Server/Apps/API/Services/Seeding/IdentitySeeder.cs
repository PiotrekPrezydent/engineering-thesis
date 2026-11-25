using Dara.Server.Modules.Identity.Application;
using Dara.Server.Modules.Identity.Application.CreateUser;

namespace Dara.Server.Apps.API.Services.Seeding;

public class IdentitySeeder : ISeeder
{
    private readonly IIdentityModule _identityModule;
    private readonly ILogger<IdentitySeeder> _logger;

    public IdentitySeeder(IIdentityModule identityModule, ILogger<IdentitySeeder> logger)
    {
        _identityModule = identityModule;
        _logger = logger;
    }

    public async Task SeedAsync(SeedData data, CancellationToken cancellationToken)
    {
        foreach (var user in data.Users)
        {
            var userId = await _identityModule.ExecuteCommandAsync<CreateUserCommand, Guid>(new CreateUserCommand(user.Identifier));
            data.UserIds.Add(user.Identifier, userId);
        }

        _logger.LogInformation("Seeded {Count} users", data.UserIds.Count);
    }
}
