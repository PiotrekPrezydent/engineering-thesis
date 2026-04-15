namespace Dara.Apps.Server.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddSignalR();
        
        //core inra
        // CoreModule core = new();
        // core.AddToServices(builder.Services);
        
        //access module
        // AccessManagmentModule mod = new();
        // mod.AddToServices(builder.Services);
        
        //builder.Services.AddTransient<AuthHub>();

        var app = builder.Build();
        //app.MapHub<AuthHub>("auth");

        app.Run();
    }
}


