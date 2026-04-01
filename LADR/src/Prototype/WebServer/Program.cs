using LADR.Prototype.WebServer;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddSignalR();
        var app = builder.Build();

        app.MapHub<ClientsHub>("clients");
        
        Console.WriteLine("Write gemini api key"); 
        var key = Console.ReadLine();
        SKManager manager = new SKManager(key);
        
        ClientsHub.manager = manager;

        app.Run();
    }
}
