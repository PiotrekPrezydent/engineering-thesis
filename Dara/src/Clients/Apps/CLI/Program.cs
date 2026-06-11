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
}