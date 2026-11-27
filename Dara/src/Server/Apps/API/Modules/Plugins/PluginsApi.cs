using Dara.Server.Apps.API.Common.Seeding;

namespace Dara.Server.Apps.API.Modules.Plugins;

// what the host adds for the plugins module; its hub methods are in AppHub.Plugins.cs, its hub notifications in
// Notifications/ (registered by the scan of the assembly)
public static class PluginsApi
{
    public static void AddPluginsApi(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ISeeder, PluginsSeeder>();
    }
}
