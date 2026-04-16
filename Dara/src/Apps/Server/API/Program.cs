using Dara.Modules.RpcGateway.Configuration;

namespace Dara.Apps.Server.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddRpcGatewayModule();

        var app = builder.Build();

        app.UseGatewayModule();

        app.Run();
    }
}


