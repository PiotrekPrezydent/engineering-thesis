using Dara.Server.Apps.API.Common.Seeding;

namespace Dara.Server.Apps.API.Modules.Plugins;

// what the host adds for the plugins module; its http endpoints are in PluginsEndpoints.cs, its hub notifications
// in Notifications/ (registered by the scan of the assembly)
public static class PluginsApi
{
    public static void AddPluginsApi(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ISeeder, PluginsSeeder>();
    }

    public static void MapPluginsApi(this WebApplication app)
    {
        app.MapPluginsEndpoints();
    }
}
