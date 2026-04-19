using Dara.BuildingBlocks.Domain.Commands;
using Dara.BuildingBlocks.Domain.Exceptions;
using Dara.Modules.RpcGateway.Contracts;
using Dara.Modules.RpcGateway.Domain;
using Dara.Shared.Common.Logging;
using Dara.Shared.Contracts;
using Dara.Shared.Contracts.Communication;
using Dara.Shared.Contracts.Connection;
using Microsoft.AspNetCore.SignalR;

namespace Dara.Modules.RpcGateway.Infrastructure;

public class AppHub : Hub<IAppHubClient>, IAppHub
{
    private readonly IApplicationCommandDispatcher _applicationCommandDispatcher;
    private readonly ConsoleLogger _consoleLogger;
    
    public AppHub(IApplicationCommandDispatcher applicationCommandDispatcher)
    {
        _consoleLogger = new ConsoleLogger(this);
        _applicationCommandDispatcher = applicationCommandDispatcher;
    }
    
    public override async Task OnConnectedAsync()
    {
        string id = Context.ConnectionId;
        string ip = Context.GetHttpContext()!.Connection.RemoteIpAddress!.ToString();

        try
        {
            var command = new SetConnectionEstablishedCommand(id,ip);
            var result = await _applicationCommandDispatcher.DispatchAsync<SetConnectionEstablishedCommand, SetConnectionEstablishedCommandResult>(command);
            
        }
        catch (BaseDomainException ex)
        {
            ex.PrintBuildedMessage();
            
            return;
        }


        
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        string id = Context.ConnectionId;

        try
        {
            var command = new SetConnectionLostCommand(id);
            var result = await _applicationCommandDispatcher
                .DispatchAsync<SetConnectionLostCommand, SetConnectionLostCommandResult>(command);
        }
        catch (BaseDomainException ex)
        {
            ex.PrintBuildedMessage();

            return;
        }


        await base.OnDisconnectedAsync(exception);
    }

    public async Task BroadcastMessageAsync(MessageDto message)
    {
        await Clients.All.ReceiveMessageAsync(message);
    }

    public async Task BroadcastMessageForConnectionsAsync(MessageDto message, params string[] connectionIds)
    {
        await Clients.Clients(connectionIds).ReceiveMessageAsync(message);
    }

    public async Task BrodcastMessageForIpConnectionsAsync(MessageDto message, string connectionIp)
    {
        var command = new GetIpConnectionsCommand(connectionIp);
        var result = await _applicationCommandDispatcher.DispatchAsync<GetIpConnectionsCommand, GetIpConnectionsCommandResult>(command);
        
        await Clients.Clients(result.ConnectionIds).ReceiveMessageAsync(message);
    }

    public Task<ResponseDto> TryBecomeClientAsync(RegisterClientRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto> TryExitClientAsync(LeaveClientRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto> TryChangeClientNameAsync(ChangeClientNameRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto> TryChangeClientAuthTokenAsync(ChangeClientAuthTokenRequest request)
    {
        throw new NotImplementedException();
    }

    public Task SendMessageToClientAsync(ClientDto client, MessageDto message)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto> TryJoinGroupAsync(JoinGroupRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto> TryLeaveGroupAsync(LeaveGroupRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto> TryCreateGroupAsync(CreateGroupRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto> TryChangeGroupNameAsync(ChangeGroupNameRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto> TryChangeGroupCodeAsync(ChangeGroupCodeRequest request)
    {
        throw new NotImplementedException();
    }
}