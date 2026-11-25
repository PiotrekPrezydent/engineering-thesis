using Dara.Server.BuildingBlocks.Domain.Exceptions;
using Dara.Server.Modules.Plugins.Domain.PluginOwners.Rules;
using Dara.Server.Modules.Plugins.Domain.Plugins.Events;
using static Dara.Server.Modules.Plugins.UnitTests.PluginSamples;

namespace Dara.Server.Modules.Plugins.UnitTests.PluginOwners;

public class PluginOwnerRulesTests
{
    [Fact]
    public void PluginWithTheSameHashCannotBeAddedTwice()
    {
        var owner = Owner();
        var files = owner.AddSample(name: "Files", hash: 'a');

        owner.AddSample(name: "Camera", hash: 'b', owned: [files]);

        AssertBroken<PluginCannotBeAddedTwiceRule>(() => owner.AddSample(name: "Files copy", hash: 'a', owned: [files]));
    }

    [Fact]
    public void PluginsOfOtherOwnersDoNotCount()
    {
        var stranger = Owner().AddSample(name: "Files", hash: 'a');

        var plugin = Owner().AddSample(name: "Files", hash: 'a', owned: [stranger]);

        Assert.NotEqual(stranger.OwnerId, plugin.OwnerId);
    }

    [Fact]
    public void NewHashReplacesPluginOnlyAfterTheOldOneIsRemoved()
    {
        var owner = Owner();
        var old = owner.AddSample(name: "Files", hash: 'a');

        AssertBroken<PluginNameMustBeUniqueForOwnerRule>(() => owner.AddSample(name: "Files", hash: 'b', owned: [old]));

        owner.RemovePlugin([old], old);
        var replacement = owner.AddSample(name: "Files", hash: 'b', owned: []);

        Assert.Equal("Files", replacement.Name);
    }

    [Fact]
    public void CurrentPluginIsRemovedWithEvent()
    {
        var owner = Owner();
        var plugin = owner.AddSample();

        owner.RemovePlugin([plugin], plugin);

        var removed = Assert.IsType<PluginRemovedDomainEvent>(plugin.DomainEvents.Last());
        Assert.Equal(plugin.Id, removed.PluginId);
        Assert.Equal(owner.Id, removed.PluginOwnerId);
    }

    [Fact]
    public void NonActualPluginCannotBeRemoved()
    {
        var owner = Owner();
        var current = owner.AddSample(name: "Files", hash: 'a');
        var gone = owner.AddSample(name: "Camera", hash: 'b');
        var stranger = Owner().AddSample(name: "Clock", hash: 'c');

        AssertBroken<NonActualPluginCannotBeRemovedRule>(() => owner.RemovePlugin([current], gone));
        AssertBroken<NonActualPluginCannotBeRemovedRule>(() => owner.RemovePlugin([current, stranger], stranger));
    }

    private static void AssertBroken<TRule>(Action action)
    {
        var exception = Assert.Throws<BuisnessRuleValidationException>(action);
        Assert.IsType<TRule>(exception.BrokenRule);
    }
}
