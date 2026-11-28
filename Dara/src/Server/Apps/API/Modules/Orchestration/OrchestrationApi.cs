using Dara.Server.Apps.API.Common.Seeding;

namespace Dara.Server.Apps.API.Modules.Orchestration;

// what the host adds for the orchestration module; its hub methods are in AppHub.Orchestration.cs, its hub
// notifications in Notifications/ (registered by the scan of the assembly)
public static class OrchestrationApi
{
    public static void AddOrchestrationApi(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ISeeder, OrchestrationSeeder>();
        builder.Services.AddScoped<GroupOwnerResolver>();
    }

    public static void MapOrchestrationApi(this WebApplication app)
    {
        app.MapOrchestrationEndpoints();
    }
}
