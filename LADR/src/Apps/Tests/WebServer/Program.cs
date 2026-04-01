using LADR.Tests.WebServer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
var app = builder.Build();
    
    
app.MapHub<AnyHub>("/hub");

app.Urls.Add("http://*:5000");

Console.WriteLine("start");

app.Run();