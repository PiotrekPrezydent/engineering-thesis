using Dara.Apps.Server.API.AppHubs;
using Dara.Modules.Configuration;

namespace Dara.Apps.Server.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddSignalR();

        builder.Services.AddBuildingBlocksInfraDispatchers();
        builder.Services.AddCommunicationModule();

        var app = builder.Build();
        
        app.MapHub<AppHub>("/app");

        app.Run();
    }
}


