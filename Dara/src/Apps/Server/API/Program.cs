using Dara.BuildingBlocks.Domain.Commands;
using Dara.BuildingBlocks.Domain.Events;
using Dara.BuildingBlocks.Infrastructure;
using Dara.Modules.RpcGateway.Configuration;

namespace Dara.Apps.Server.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddSingleton<IApplicationCommandDispatcher, ApplicationCommandDispatcher>();
        builder.Services.AddSingleton<IDomainEventDispatcher, DomainEventDispatcher>();

        builder.Services.AddRpcGatewayModule();

        var app = builder.Build();

        app.UseGatewayModule();

        app.Run();
    }
}


