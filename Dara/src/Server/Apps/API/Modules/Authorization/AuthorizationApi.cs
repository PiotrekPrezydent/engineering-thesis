using Dara.Server.Apps.API.Common.Seeding;

namespace Dara.Server.Apps.API.Modules.Authorization;

// what the host adds for the authorization module: its http endpoints and the seeder of the administrator of the
// server; the check of marked endpoints and hub methods is in Common/Authorization and Common/Hubs/AuthorizationHubFilter
public static class AuthorizationApi
{
    public static void AddAuthorizationApi(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ISeeder, AuthorizationSeeder>();
    }

    public static void MapAuthorizationApi(this WebApplication app)
    {
        app.MapAuthorizationEndpoints();
    }
}
