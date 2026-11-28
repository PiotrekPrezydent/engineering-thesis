using System.Net;
using Dara.Clients.Core.Api;
using Dara.Server.Apps.API.Common.FunctionCalls;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.Modules.Orchestration.Integration;
using Dara.Server.SystemTests.Hosting;
using Dara.Shared.Contracts.FunctionCalls;

namespace Dara.Server.SystemTests.ClientServer;

// a call of orchestration goes over the hub to its client only, the result comes back over http from that client and
// reaches the port; until TASK-ENG-013 publishes the event, the tests publish it on the bus themselves
public class FunctionCallTests
{
    [Fact]
    public async Task ACallReachesOnlyItsClientAndOnlyThatClientAnswersItOnce()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken);
        var laptop = await host.ConnectAsync("called-laptop", cancellationToken);
        var phone = await host.ConnectAsync("other-phone", cancellationToken);

        var call = await PublishCallAsync(host, laptop.ClientId);
        var request = await laptop.Events.WaitForAsync<FunctionCallRequest>(r => r.CallId == call.CallId, "call at the laptop");
        var second = await PublishCallAsync(host, phone.ClientId);
        await phone.Events.WaitForAsync<FunctionCallRequest>(r => r.CallId == second.CallId, "second call at the phone");

        Assert.Equal(("Camera", new string('c', 64), "TakePhoto", """{"quality":"high"}"""),
            (request.PluginName, request.PluginHash, request.FunctionName, request.ArgumentsJson));
        Assert.Equal([second.CallId], phone.Events.Received<FunctionCallRequest>().Select(r => r.CallId));

        var foreign = await Assert.ThrowsAsync<DaraApiException>(() =>
            phone.Api.SubmitFunctionCallResultAsync(call.CallId, new FunctionCallResultRequest(FunctionCallOutcome.Succeeded, "\"stolen\"", null), cancellationToken));
        var invalid = await Assert.ThrowsAsync<DaraApiException>(() =>
            laptop.Api.SubmitFunctionCallResultAsync(call.CallId, new FunctionCallResultRequest(FunctionCallOutcome.Succeeded, "not json", null), cancellationToken));
        await laptop.Api.SubmitFunctionCallResultAsync(call.CallId, new FunctionCallResultRequest(FunctionCallOutcome.Succeeded, "\"photo\"", null), cancellationToken);
        var repeated = await Assert.ThrowsAsync<DaraApiException>(() =>
            laptop.Api.SubmitFunctionCallResultAsync(call.CallId, new FunctionCallResultRequest(FunctionCallOutcome.Failed, null, "again"), cancellationToken));

        Assert.Equal((HttpStatusCode.NotFound, FunctionCallErrorCodes.UnknownCall), (foreign.StatusCode, foreign.Code));
        Assert.Equal((HttpStatusCode.BadRequest, FunctionCallErrorCodes.InvalidResult), (invalid.StatusCode, invalid.Code));
        Assert.Equal((HttpStatusCode.Conflict, FunctionCallErrorCodes.AlreadyCompleted), (repeated.StatusCode, repeated.Code));
        var result = Assert.Single(host.FunctionCallResults.Results);
        Assert.Equal((call.CallId, laptop.ClientId, FunctionCallResultOutcome.Succeeded, "\"photo\""),
            (result.CallId, result.ClientId, result.Outcome, result.ResultJson));
    }

    [Fact]
    public async Task ACallForAnOfflineClientIsNotDelivered()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var host = await AppHubTestHost.StartAsync(cancellationToken);
        var laptop = await host.ConnectAsync("offline-laptop", cancellationToken);
        await laptop.Connection.StopAsync(cancellationToken);
        await ModulesHost.WaitUntilAsync(() => Task.FromResult(!host.Service<Apps.API.Common.Presence.IClientPresence>().IsOnline(laptop.ClientId)),
            "laptop offline");

        var call = await PublishCallAsync(host, laptop.ClientId);

        await ModulesHost.WaitUntilAsync(() => Task.FromResult(host.FunctionCallResults.Results.Any(r => r.CallId == call.CallId)), "not delivered");
        var result = Assert.Single(host.FunctionCallResults.Results);
        Assert.Equal((laptop.ClientId, FunctionCallResultOutcome.NotDelivered), (result.ClientId, result.Outcome));
        Assert.Empty(laptop.Events.Received<FunctionCallRequest>());
        var late = await Assert.ThrowsAsync<DaraApiException>(() =>
            laptop.Api.SubmitFunctionCallResultAsync(call.CallId, new FunctionCallResultRequest(FunctionCallOutcome.NotExecuted, null, "late"), cancellationToken));
        Assert.Equal(HttpStatusCode.NotFound, late.StatusCode);
    }

    private static async Task<FunctionCallRequestedIntegrationEvent> PublishCallAsync(AppHubTestHost host, Guid clientId)
    {
        var call = new FunctionCallRequestedIntegrationEvent(Guid.NewGuid(), DateTime.UtcNow, Guid.NewGuid(), Guid.NewGuid(), clientId,
            "Camera", new string('c', 64), "TakePhoto", """{"quality":"high"}""", DateTime.UtcNow.AddMinutes(1));
        await host.Service<IEventBus>().PublishAsync(call);
        return call;
    }
}
