using Dara.BuildingBlocks.Infrastructure.CompositionRoots;
using Dara.BuildingBlocks.Infrastructure.Logging;
using Dara.Server.Apps.API.AppHubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging.Console;

namespace Dara.Server.Apps.API
{
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
                options.FormatterName = nameof(DaraLogFormatter); 
            });
            builder.Logging.AddConsoleFormatter<DaraLogFormatter, ConsoleFormatterOptions>();
            
            BuildingBlocksCompositionRoot.Initialize(builder.Services);
            //ConnectionsCompositionRoot.Initialize(builder.Services);
            
            var app = builder.Build();
        
            app.MapHub<AppHub>("/app");

            app.Run();
        }
    }
}


