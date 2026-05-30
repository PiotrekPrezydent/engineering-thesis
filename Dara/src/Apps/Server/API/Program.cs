using Dara.Apps.Server.API.AppHubs;

using Dara.BuildingBlocks.Infrastructure.Configuration;
using Dara.Modules.Connections.Infrastructure.Configuration;

namespace Dara.Apps.Server.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSignalR();
            
            BuildingBlocksCompositionRoot.Initialize(builder.Services);
            ConnectionsCompositionRoot.Initialize(builder.Services);
            
            var app = builder.Build();
        
            app.MapHub<AppHub>("/app");

            app.Run();
        }
    }
}


