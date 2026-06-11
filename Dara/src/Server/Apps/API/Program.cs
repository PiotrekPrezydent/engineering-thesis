using Dara.Server.Apps.API.Hubs;
using Dara.Server.Apps.API.Utils;
using Dara.Server.Modules.Clients.Application;
using Dara.Server.Modules.Clients.Infrastructure;
using Microsoft.AspNetCore.SignalR;

namespace Dara.Server.Apps.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
            
        builder.Services.AddSingleton<IUserIdProvider, HeaderUserIdProvider>();
        builder.Services.AddSignalR();
            
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole(options =>
        {
            //options.FormatterName = nameof(DaraLogFormatter); 
        });
        //builder.Logging.AddConsoleFormatter<DaraLogFormatter, ConsoleFormatterOptions>();
        ClientsCompositionRoot.Initialize();
        builder.Services.AddScoped<IClientsModule, ClientsModule>();
            
        var app = builder.Build();
            
        app.Use(async (context, next) =>
        { 
            if (context.Request.Path.StartsWithSegments("/app")) 
            {
                if (!context.Request.Headers.ContainsKey("X-Client-Id"))
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("No header: X-Client-Id");
                    return;
                }
            }
            await next();
        });
        
        app.MapHub<AppHub>("/app");

        app.Run();
    }
}