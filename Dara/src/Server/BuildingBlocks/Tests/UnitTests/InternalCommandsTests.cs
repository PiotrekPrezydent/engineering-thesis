using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration;
using Dara.Server.BuildingBlocks.Infrastructure.Processing.InternalCommands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.Server.BuildingBlocks.UnitTests;

public class InternalCommandsTests
{
    [Fact]
    public async Task FailingCommandIsRetriedUntilItSucceedsAndHasItsEffectOnce()
    {
        await using var module = new TestModule();
        module.Probe.FailFirst("flaky", 3);
        await module.StartAsync<InternalCommandsBackgroundWorker>();

        await module.EnqueueAsync(new EffectCommand("flaky"));
        await module.WaitUntilProcessedAsync("flaky");

        var command = await module.GetCommandAsync("flaky");
        Assert.Equal(1, await module.CountEffectsAsync("flaky"));
        Assert.Equal(3, command.Attempts);
        Assert.Contains("failure 3 of flaky", command.LastError);
    }

    [Fact]
    public async Task ProcessedCommandProcessedAgainHasNoSecondEffect()
    {
        await using var module = new TestModule();
        await module.EnqueueAsync(new EffectCommand("once"));
        var id = (await module.GetCommandAsync("once")).Id;

        for (var i = 0; i < 2; i++)
        {
            await using var scope = module.CreateAsyncScope();
            await scope.ServiceProvider.GetRequiredService<IInternalCommandProcessor>().ProcessAsync(id, CancellationToken.None);
        }

        Assert.Equal(1, await module.CountEffectsAsync("once"));
        Assert.NotNull((await module.GetCommandAsync("once")).ProcessedDate);
    }

    [Fact]
    public async Task FailedAttemptSavesNothingButTheError()
    {
        await using var module = new TestModule();
        module.Probe.FailFirst("broken", 1);
        await module.EnqueueAsync(new EffectCommand("broken"));
        var id = (await module.GetCommandAsync("broken")).Id;

        await using (var scope = module.CreateAsyncScope())
        {
            var processor = scope.ServiceProvider.GetRequiredService<IInternalCommandProcessor>();
            var error = await Assert.ThrowsAsync<InvalidOperationException>(() => processor.ProcessAsync(id, CancellationToken.None));

            await using var failureScope = module.CreateAsyncScope();
            await failureScope.ServiceProvider.GetRequiredService<IInternalCommandProcessor>().RecordFailureAsync(id, error, CancellationToken.None);
        }

        var command = await module.GetCommandAsync("broken");
        Assert.Null(command.ProcessedDate);
        Assert.Equal(1, command.Attempts);
        Assert.Equal(0, await module.CountEffectsAsync("broken"));
    }

    [Fact]
    public async Task LongCommandDoesNotHoldBackTheNextOne()
    {
        await using var module = new TestModule();
        module.Probe.Hold("long");
        await module.StartAsync<InternalCommandsBackgroundWorker>();

        await module.EnqueueAsync(new EffectCommand("long"), new EffectCommand("short"));
        await module.WaitUntilProcessedAsync("short");

        Assert.Null((await module.GetCommandAsync("long")).ProcessedDate);

        module.Probe.Release("long");
        await module.WaitUntilProcessedAsync("long");
    }

    [Fact]
    public async Task CommandsOfOneKeyRunInOrderEvenWhenTheFirstIsRetried()
    {
        await using var module = new TestModule();
        module.Probe.FailFirst("first", 2);
        await module.StartAsync<InternalCommandsBackgroundWorker>();

        await module.EnqueueAsync(
            new OrderedEffectCommand("first", "group:1"),
            new OrderedEffectCommand("second", "group:1"),
            new OrderedEffectCommand("other", "group:2"));
        await module.WaitUntilProcessedAsync("second");
        await module.WaitUntilProcessedAsync("other");

        var started = module.Probe.Started.ToList();
        Assert.True(started.LastIndexOf("first") < started.IndexOf("second"), string.Join(", ", started));
        Assert.Equal(["first", "second"], module.Probe.Finished.Where(n => n != "other"));
    }

    [Fact]
    public async Task GroupRunsNoMoreCommandsAtOnceThanItsLimit()
    {
        var options = TestModule.FastOptions with { ConcurrencyLimits = new Dictionary<string, int> { ["plan-generation"] = 2 } };
        await using var module = new TestModule(options);
        string[] names = ["a", "b", "c"];
        foreach (var name in names)
            module.Probe.Hold(name);
        await module.StartAsync<InternalCommandsBackgroundWorker>();

        await module.EnqueueAsync(names.Select(n => (ICommand)new GroupedEffectCommand(n, "plan-generation")).ToArray());
        await module.WaitUntilAsync(() => Task.FromResult(module.Probe.Started.Count == 2), "two commands of the group started");
        await Task.Delay(200, TestContext.Current.CancellationToken);

        Assert.Equal(2, module.Probe.Started.Count);

        module.Probe.ReleaseAll();
        foreach (var name in names)
            await module.WaitUntilProcessedAsync(name);

        Assert.Equal(2, module.Probe.MaxRunningInGroup("plan-generation"));
    }

    [Fact]
    public async Task QueuedCommandIsStoredOnlyWithTheSaveOfItsScope()
    {
        await using var module = new TestModule();

        await using (var scope = module.CreateAsyncScope())
        {
            await scope.ServiceProvider.GetRequiredService<IInternalCommandScheduler>().EnqueueAsync(new EffectCommand("unsaved"));
        }

        Assert.Equal(0, await module.ReadAsync(db => db.InternalCommands.CountAsync()));
    }

    [Fact]
    public async Task CommandWithAResultCannotBeQueued()
    {
        await using var module = new TestModule();
        await using var scope = module.CreateAsyncScope();
        var scheduler = scope.ServiceProvider.GetRequiredService<IInternalCommandScheduler>();

        await Assert.ThrowsAsync<InvalidOperationException>(() => scheduler.EnqueueAsync(new CommandWithResult("x")));
    }

    [Fact]
    public async Task OrderingKeyAndGroupAreColumnsAndNotData()
    {
        await using var module = new TestModule();

        await module.EnqueueAsync(new OrderedEffectCommand("keyed", "member:1"), new GroupedEffectCommand("grouped", "plan-generation"));

        var keyed = await module.GetCommandAsync("keyed");
        var grouped = await module.GetCommandAsync("grouped");
        Assert.Equal("member:1", keyed.OrderingKey);
        Assert.Equal("plan-generation", grouped.ConcurrencyGroup);
        Assert.DoesNotContain("OrderingKey", keyed.Data);
        Assert.DoesNotContain("ConcurrencyGroup", grouped.Data);
        Assert.True(keyed.EnqueuedAt < grouped.EnqueuedAt);
    }
}
