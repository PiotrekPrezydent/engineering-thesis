using System.Threading.RateLimiting;
using Dara.Server.Apps.API.Common.Seeding;

namespace Dara.Server.Apps.API.Modules.Identity;

// what the host adds for the identity module: the rate limit of the codes, the seeder and the http endpoints of
// sign-in and clients
public static class IdentityApi
{
    public static void AddIdentityApi(this WebApplicationBuilder builder)
    {
        AddClientCodeRateLimit(builder);
        builder.Services.AddScoped<ISeeder, IdentitySeeder>();
    }

    public static void MapIdentityApi(this WebApplication app)
    {
        app.MapAuthEndpoints();
        app.MapAccountClientsEndpoints();
        app.MapCurrentClientEndpoints();
    }

    // attempts of register-client and client-login per client address in a fixed window of one minute
    // (Auth:ClientCodePermitsPerMinute)
    private static void AddClientCodeRateLimit(WebApplicationBuilder builder)
    {
        var permits = builder.Configuration.GetValue("Auth:ClientCodePermitsPerMinute", 10);
        if (permits <= 0)
            throw new InvalidOperationException("Auth__ClientCodePermitsPerMinute must be positive");

        builder.Services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.AddPolicy(AuthEndpoints.ClientCodeRateLimit, context => RateLimitPartition.GetFixedWindowLimiter(
                context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = permits,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 0
                }));
        });
    }
}
