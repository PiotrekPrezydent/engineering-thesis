namespace Dara.Apps.Tests.Server;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddSignalR();
        
        
        var app = builder.Build();
        app.MapHub<AppHub>("/app");

        app.MapGet("/", () => "Hello World!");
        
 

        app.Run();
    }
}
