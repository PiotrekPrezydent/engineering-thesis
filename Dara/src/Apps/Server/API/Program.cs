using Dara.Apps.Server.API.Initializers;

namespace Dara.Apps.Server.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddSignalR();
        builder.Services.AddBuildingBlocks();
        builder.Services.AddAccessManagmentModule();
        
        builder.Services.AddSingleton<AppHub>();

        var app = builder.Build();
        app.MapHub<AppHub>("/app");

        app.Run();
    }
}


