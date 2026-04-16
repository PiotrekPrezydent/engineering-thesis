using Dara.BuildingBlocks.Domain.Commands;
using Dara.Modules.AccessManagment.Application.Clients;
using Dara.Modules.AccessManagment.Application.Devices;
using Dara.Shared.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace Dara.Apps.Server.API;

public class AppHub : Hub<IAppClient>, IAppHub
{
    private readonly IApplicationCommandDispatcher  _commandDispatcher;
    private readonly Dictionary<string, Guid> _connectionClientsMap;
    private readonly Dictionary<string, Guid> _devicesIpMap;

    public AppHub(IApplicationCommandDispatcher commandDispatcher)
    {
        _commandDispatcher = commandDispatcher;
        _connectionClientsMap = new();
        _devicesIpMap = new();
        Console.WriteLine($"APP HUB CREATED");
    }
    //client name = NoName-Device.Clients.Length+1
    //device name = NoName-Ip_address
    
    public override async Task OnConnectedAsync()
    {
        Console.WriteLine($"{Context.ConnectionId} connected");
        
        string connectionId = Context.ConnectionId;
        string? userId = Context.UserIdentifier;
        Guid clientId;
        Guid deviceId;
        try
        {
            var command = new AddClientCommand("Unnamed Client"+connectionId, connectionId);
            var result = await _commandDispatcher.DispatchAsync<AddClientCommand,AddClientCommandResult>(command);
            clientId = result.ClientId;
            //add this client to domain
            _connectionClientsMap.Add(connectionId, clientId);
        }
        catch (Exception e)
        {
            Console.WriteLine("###################################");
            Console.WriteLine(e);
            Console.WriteLine("###################################");
            return;
        }
        
        var httpContext = Context.GetHttpContext()!;
        string ipAddress = httpContext.Connection.RemoteIpAddress!.ToString();
        
        if (!_devicesIpMap.TryGetValue(ipAddress, out deviceId))
        {
            try
            {
                var command = new AddDeviceCommand("Unnamed Device"+connectionId, connectionId);
                var result = await  _commandDispatcher.DispatchAsync<AddDeviceCommand, AddDeviceCommandResult>(command);
                deviceId = result.DeviceId;
                _devicesIpMap.Add(ipAddress, deviceId);
            }
            catch (Exception e)
            {
                Console.WriteLine("###################################");
                Console.WriteLine(e);
                Console.WriteLine("###################################");
            }
        }
        //add client to device
        try
        {
            var command = new RegisterClientDeviceCommand(clientId, deviceId);
            var result = await _commandDispatcher.DispatchAsync<RegisterClientDeviceCommand, RegisterClientDeviceCommandResult>(command);

        }
        catch (Exception e)
        {
            Console.WriteLine("###################################");
            Console.WriteLine(e);
            Console.WriteLine("###################################");
        }
        
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine($"{Context.ConnectionId} disconnected");
        string connectionId = Context.ConnectionId;
        string? userId = Context.UserIdentifier;
        
        Guid clientId = _connectionClientsMap[connectionId];
        //remvoe client from user and device
        
        await base.OnDisconnectedAsync(exception);
    }
}