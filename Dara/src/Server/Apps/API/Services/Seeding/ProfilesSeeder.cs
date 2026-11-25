using Dara.Server.Modules.Profiles.Application;
using Dara.Server.Modules.Profiles.Application.ChangeProfileName;
using Dara.Server.Modules.Profiles.Application.GetProfile;

namespace Dara.Server.Apps.API.Services.Seeding;

public class ProfilesSeeder : ISeeder
{
    private readonly IProfilesModule _profilesModule;
    private readonly SeedEnvironment _environment;
    private readonly ILogger<ProfilesSeeder> _logger;

    public ProfilesSeeder(IProfilesModule profilesModule, SeedEnvironment environment, ILogger<ProfilesSeeder> logger)
    {
        _profilesModule = profilesModule;
        _environment = environment;
        _logger = logger;
    }

    public async Task SeedAsync(SeedData data, CancellationToken cancellationToken)
    {
        // default profiles are created from the new user event of identity
        var userIds = data.UserIds.Values.ToList();
        await _environment.WaitUntilAsync(async () =>
        {
            var profiles = await _profilesModule.ExecuteQueryAsync<GetProfilesQuery, List<ProfileDto>>(new GetProfilesQuery(userIds));
            return profiles.Count == userIds.Count;
        }, "default profiles of seeded users", cancellationToken);

        foreach (var user in data.Users)
            await _profilesModule.ExecuteCommandAsync(new ChangeProfileNameCommand(data.UserIds[user.Identifier], user.ProfileName));

        _logger.LogInformation("Seeded {Count} profile names", data.Users.Count);
    }
}
