using Dara.Server.BuildingBlocks.Domain.Exceptions;
using Dara.Server.Modules.Plugins.Domain.PluginConfigurations;
using Dara.Server.Modules.Plugins.Domain.PluginConfigurations.Events;
using Dara.Server.Modules.Plugins.Domain.PluginConfigurations.Rules;
using Dara.Server.Modules.Plugins.Domain.PluginOwners;
using Dara.Server.Modules.Plugins.Domain.Plugins;

namespace Dara.Server.Modules.Plugins.UnitTests.PluginConfigurations;

public class PluginConfigurationTests
{
    private readonly PluginOwnerId _owner = new(Guid.NewGuid());
    private readonly PluginFunctionId _save = new(Guid.NewGuid());
    private readonly PluginFunctionId _load = new(Guid.NewGuid());

    private PluginConfiguration Create() => PluginConfiguration.Create(_owner, new PluginId(Guid.NewGuid()), [_save, _load]);

    [Fact]
    public void NewConfigurationIsEnabledWithAllFunctionsAtVersionOne()
    {
        var configuration = Create();

        Assert.True(configuration.IsEnabled);
        Assert.Equal(1, configuration.Version);
        Assert.Equal(new[] { _save, _load }, configuration.GetAvailableFunctionIds());
        Assert.Empty(configuration.DomainEvents);
    }

    [Fact]
    public void DisabledPluginHasNoAvailableFunctions()
    {
        var configuration = Create();

        configuration.DisablePlugin(_owner);

        Assert.False(configuration.IsEnabled);
        Assert.Empty(configuration.GetAvailableFunctionIds());
        var disabled = Assert.IsType<PluginDisabledDomainEvent>(Assert.Single(configuration.DomainEvents));
        Assert.Equal(2, disabled.Version);
        Assert.Equal(configuration.PluginId, disabled.PluginId);
    }

    [Fact]
    public void OnlyActivePluginCanBeDisabled()
    {
        var configuration = Create();
        configuration.DisablePlugin(_owner);

        AssertBroken<OnlyActivePluginCanBeDisabledRule>(() => configuration.DisablePlugin(_owner));
    }

    [Fact]
    public void OnlyNonActivePluginCanBeEnabled()
    {
        var configuration = Create();

        AssertBroken<OnlyNonActivePluginCanBeEnabledRule>(() => configuration.EnablePlugin(_owner));

        configuration.DisablePlugin(_owner);
        configuration.EnablePlugin(_owner);

        Assert.True(configuration.IsEnabled);
        Assert.IsType<PluginEnabledDomainEvent>(configuration.DomainEvents.Last());
    }

    [Fact]
    public void FunctionIsFoundByItsFunctionId()
    {
        var configuration = Create();

        configuration.DisableFunction(_owner, _load);

        Assert.Equal(new[] { _save }, configuration.GetAvailableFunctionIds());
        var disabled = Assert.IsType<PluginFunctionDisabledDomainEvent>(Assert.Single(configuration.DomainEvents));
        Assert.Equal(_load, disabled.PluginFunctionId);
        Assert.Equal(2, disabled.Version);
    }

    [Fact]
    public void OnlyActiveFunctionCanBeDisabled()
    {
        var configuration = Create();
        configuration.DisableFunction(_owner, _save);

        AssertBroken<OnlyActivePluginFunctionCanBeDisabledRule>(() => configuration.DisableFunction(_owner, _save));
    }

    [Fact]
    public void OnlyNonActiveFunctionCanBeEnabled()
    {
        var configuration = Create();

        AssertBroken<OnlyNonActivePluginFunctionCanBeEnabledRule>(() => configuration.EnableFunction(_owner, _save));

        configuration.DisableFunction(_owner, _save);
        configuration.EnableFunction(_owner, _save);

        Assert.Equal(new[] { _save, _load }, configuration.GetAvailableFunctionIds());
        Assert.IsType<PluginFunctionEnabledDomainEvent>(configuration.DomainEvents.Last());
    }

    [Fact]
    public void FunctionMustBelongToPlugin()
    {
        var configuration = Create();

        AssertBroken<FunctionMustBelongToPluginRule>(() => configuration.DisableFunction(_owner, new PluginFunctionId(Guid.NewGuid())));
        AssertBroken<FunctionMustBelongToPluginRule>(() => configuration.EnableFunction(_owner, new PluginFunctionId(Guid.NewGuid())));
    }

    [Fact]
    public void OnlyOwnerCanConfigurePlugin()
    {
        var configuration = Create();
        var stranger = new PluginOwnerId(Guid.NewGuid());

        AssertBroken<OnlyPluginOwnerCanConfigurePluginRule>(() => configuration.DisablePlugin(stranger));
        AssertBroken<OnlyPluginOwnerCanConfigurePluginRule>(() => configuration.DisableFunction(stranger, _save));
        Assert.Equal(1, configuration.Version);
    }

    [Fact]
    public void VersionGrowsWithEveryChangeAndRemoval()
    {
        var configuration = Create();

        configuration.DisableFunction(_owner, _save);
        configuration.DisablePlugin(_owner);
        configuration.EnablePlugin(_owner);
        configuration.Remove();

        Assert.Equal(5, configuration.Version);
        var removed = Assert.IsType<PluginConfigurationRemovedDomainEvent>(configuration.DomainEvents.Last());
        Assert.Equal(5, removed.Version);
        Assert.Equal(_owner, removed.PluginOwnerId);
    }

    private static void AssertBroken<TRule>(Action action)
    {
        var exception = Assert.Throws<BuisnessRuleValidationException>(action);
        Assert.IsType<TRule>(exception.BrokenRule);
    }
}
