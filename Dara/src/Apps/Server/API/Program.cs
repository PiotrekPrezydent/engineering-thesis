using Dara.Apps.Server.API.AppHubs;
using Dara.BuildingBlocks.Infrastructure;
using Dara.BuildingBlocks.Infrastructure.Commands;
using Dara.BuildingBlocks.Infrastructure.Domain;
using Dara.Modules.Connections.Application;
using Dara.Modules.Connections.Infrastructure;
using Dara.Modules.Groups.Application;
using Dara.Modules.Groups.Infrastructure;

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
            Module groupsModule = new(new GroupsApplicationLayer(), new GroupsInfrastructureLayer());

            builder.Services.AddModule(connectionsModule);
            builder.Services.AddModule(groupsModule);

            var app = builder.Build();
        
            app.MapHub<AppHub>("/app");

            app.Run();
        }
    }
}


