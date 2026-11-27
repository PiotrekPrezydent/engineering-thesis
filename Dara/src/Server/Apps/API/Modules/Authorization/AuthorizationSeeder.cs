using Dara.Server.Apps.API.Common.Seeding;
using Dara.Server.Modules.Authorization.Application;
using Dara.Server.Modules.Authorization.Application.RoleAssignments.AssignServerAdmin;

namespace Dara.Server.Apps.API.Modules.Authorization;

// the administrator of the server: Authorization.ServerAdmin on every resource for the panel client of the account
// SeedData.ServerAdmin (so it acts through the panel, a client has only its own roles); the only role above all
// others, nobody grants it through the api
public class AuthorizationSeeder : ISeeder
{
    private readonly IAuthorizationModule _authorizationModule;
    private readonly ILogger<AuthorizationSeeder> _logger;

    public AuthorizationSeeder(IAuthorizationModule authorizationModule, ILogger<AuthorizationSeeder> logger)
    {
        _authorizationModule = authorizationModule;
        _logger = logger;
    }

    public async Task SeedAsync(SeedData data, CancellationToken cancellationToken)
    {
        await _authorizationModule.ExecuteCommandAsync(new AssignServerAdminCommand(data.PanelClientIds[data.ServerAdmin]));
        _logger.LogInformation("Seeded the administrator of the server: the account {Account}", data.ServerAdmin);
    }
}
