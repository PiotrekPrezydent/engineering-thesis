using Microsoft.AspNetCore.SignalR;

namespace LADR.Prototype.WebServer;

public class ClientsHub : Hub
{
    public static SKManager manager;
    
    public List<KeyValuePair<string, List<object>>> ClientsPlugins = new();
    
    public override Task OnConnectedAsync()
    {
        Console.WriteLine($"connection ID: {Context.ConnectionId}");
        return base.OnConnectedAsync();
    }
    
    public override Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine($"disconnected ID: {Context.ConnectionId}");
        return base.OnDisconnectedAsync(exception);
    }
    
    public async Task JoinRoom(string roomId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        
        await Clients.Group(roomId).SendAsync("ReceiveMessage", $"somebody joined room: {roomId}");
        Console.WriteLine($"SERWER: {Context.ConnectionId} joined {roomId}");
    }
    
    public async Task SendMessageToRoom(string roomId, string message)
    {
        Console.WriteLine($"Massage in room {roomId}: {message}");
        
        await Clients.Group(roomId).SendAsync("ReceiveMessage", message);
    }

    public async Task AddPlugins(string roomId, List<object> plugins)
    {
        Console.WriteLine($"Adding plugins in {roomId} for {Context.ConnectionId}");
        ClientsPlugins.Add(new KeyValuePair<string, List<object>>(Context.ConnectionId, plugins));
        
        await Clients.Group(roomId).SendAsync("PluginsAdded", plugins);
    }
    
    public async Task SendPrompt(string roomId, string message)
    {
        Console.WriteLine("Received prompt message: " + message);
        await SendMessageToRoom(roomId, "server received prompt message: " + message);
    }
}