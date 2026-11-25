using System.Text.Json;
using Dara.Shared.Contracts.Functions;
using Dara.Shared.Contracts.Plugins;
using Dara.Shared.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace Dara.Clients.Apps.CLI;

class Program
{
    static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        await TestPlugins();
        
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole(options =>
        {
            options.FormatterName = nameof(SharedLogFormatter);
        });
            
        builder.Logging.AddConsoleFormatter<SharedLogFormatter, ConsoleFormatterOptions>();
        builder.Services.AddTransient<CLIClient>();
            
        var host = builder.Build();
            
        var app = host.Services.GetRequiredService<CLIClient>();
        await app.RunAsync();
    }


    static async Task TestPlugins()
    {
        PluginManifest data = new(
            "SomeName", 
            "SomeDesc", 
            "1.0.0",
            new string('0', 64),
            [
                new FunctionDescription("Fun1", "fun1desc",
                [
                    new PropertyDescription("parnam", new TypeDescription(TypeKind.String, Description: "pardesc"), true),
                    new PropertyDescription("nm", new TypeDescription(TypeKind.Integer, Default: "5"), false)
                ],
                new TypeDescription(TypeKind.String))
            ]);
        
        var json = JsonSerializer.Serialize(data);
        Console.WriteLine(json);
        
        await Task.Delay(TimeSpan.FromSeconds(5));
    }
}