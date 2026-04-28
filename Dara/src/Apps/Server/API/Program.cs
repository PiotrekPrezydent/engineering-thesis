using Dara.Apps.Server.API.AppHubs;
using Dara.BuildingBlocks.Infrastructure;
using Dara.BuildingBlocks.Infrastructure.Abstractions;

namespace Dara.Apps.Server.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSignalR();

            builder.Services.AddSingleton<IDomainEventDispatcher, DomainEventDispatcher>();
            builder.Services.AddSingleton<IModuleCommandRunner, ModuleCommandRunner>();
            builder.Services.AddSingleton<IIntegrationDispatcher, IntegrationDispatcher>();
            var app = builder.Build();
        
            app.MapHub<AppHub>("/app");

            app.Run();
        }
    }
}


