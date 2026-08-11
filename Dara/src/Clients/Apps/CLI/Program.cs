using System.Collections.Immutable;
using System.Text.Json;
using Dara.Shared.Contracts;
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
        PluginData data = new(
            "SomeName", 
            "SomeDesc", 
            [
                new PluginFunctionData("Fun1", "fun1desc", "rettype", 
                [
                    new PluginFunctionParameterData("parnam", "pardesc", "party"),
                    new PluginFunctionParameterData("nm", "desc", "ttd")
                ])
            ]);
        
        var json = JsonSerializer.Serialize(data,typeof(PluginData));
        Console.WriteLine(json);
        
        await Task.Delay(TimeSpan.FromSeconds(5));
    }
}