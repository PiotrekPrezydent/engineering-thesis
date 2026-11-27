using Dara.Server.Apps.API.Common.Seeding;

namespace Dara.Server.Apps.API.Modules.Groups;

// what the host adds for the groups module; its hub methods are in AppHub.Groups.cs, its hub notifications in
// Notifications/ (registered by the scan of the assembly)
public static class GroupsApi
{
    public static void AddGroupsApi(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ISeeder, GroupsSeeder>();
    }
}
