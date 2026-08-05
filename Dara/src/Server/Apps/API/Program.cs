using Dara.Server.Apps.API.Hubs;
using Dara.Server.Apps.API.Utils;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration.CompositionRoot;
using Dara.Server.Modules.Groups.Application;
using Dara.Server.Modules.Groups.Infrastructure;
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
        var modulesRoots = new IModuleCompositionRoot[]
        {
            new GroupModuleCompositionRoot()
        };
        
        foreach (var module in modulesRoots)
            module.Initialize(builder.Services);
        
        
        var app = builder.Build();

        // using (var scope = app.Services.CreateScope())
        // {
        //     var mod = scope.ServiceProvider.GetRequiredService<IGroupModule>();
        // }
        
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
