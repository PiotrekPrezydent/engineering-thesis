using Dara.Apps.Server.API.AppHubs;
using Dara.BuildingBlocks.Infrastructure;
using Dara.BuildingBlocks.Infrastructure.Commands;
using Dara.BuildingBlocks.Infrastructure.Domain;
using Dara.Modules.Connections.Application;
using Dara.Modules.Connections.Infrastructure;

namespace Dara.Apps.Server.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSignalR();

            builder.Services.AddSingleton<IDomainEventDispatcher, DomainEventDispatcher>();
            builder.Services.AddSingleton<IApplicationCommandDispatcher, ApplicationCommandDispatcher>();

            Module connectionsModule = new(new ConnectionsApplicationLayer(), new ConnectionsInfrastructureLayer());

            builder.Services.AddModule(connectionsModule);

            var app = builder.Build();
        
            app.MapHub<AppHub>("/app");

            app.Run();
        }
    }
}


