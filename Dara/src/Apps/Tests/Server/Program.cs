using Dara.BuildingBlocks.Domain.Commands;
using Dara.BuildingBlocks.Domain.Events;
using Dara.BuildingBlocks.Domain.Events.Abstractions;
using Dara.BuildingBlocks.Infrastructure;

namespace Dara.Apps.Tests.Server;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddSignalR();
        
        builder.Services.AddScoped<IApplicationCommandDispatcher, ApplicationCommandDispatcher>();
        builder.Services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        

        builder.Services.AddTransient<TestApp>();
        
        
        var app = builder.Build();
        using (var scope = app.Services.CreateScope())
        {
            var test = scope.ServiceProvider.GetRequiredService<TestApp>();
            test.Test();    
        }
        
        //app.MapHub<AppHub>("/app");

        //app.MapGet("/", () => "Hello World!");
        

        app.Run();
    }
}

public class TestApp
{
    private IApplicationCommandDispatcher _dispatcher;
    
    public TestApp(IApplicationCommandDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    public void Test()
    {
    }
}
