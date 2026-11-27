using System.Text.RegularExpressions;
using Dara.Server.Modules.Orchestration.Domain.AgentRuns;
using Dara.Server.Modules.Orchestration.Domain.Participants;
using Dara.Server.Modules.Orchestration.Domain.Participants.Functions;
using static Dara.Server.Modules.Orchestration.UnitTests.OrchestrationSamples;

namespace Dara.Server.Modules.Orchestration.UnitTests.AgentRuns;

public class ToolAliasBuilderTests
{
    private static readonly Regex ProviderLimits = new("^[A-Za-z][A-Za-z0-9_]{0,63}$");

    [Theory]
    [InlineData("Bob's phone", "BobsPhone")]
    [InlineData("Łukasz Żółć", "LukaszZolc")]
    [InlineData("bob-laptop.home", "BobLaptopHome")]
    [InlineData("Zoë_tv", "ZoeTv")]
    [InlineData("Søren", "Sren")]
    [InlineData("123 tv", "P123Tv")]
    [InlineData("   ", "P")]
    [InlineData("!!!", "P")]
    public void ParticipantNameIsCleanedForTheProviders(string participantName, string participantPart)
    {
        var map = ToolAliasBuilder.Build([Client(participantName, Ping)]);

        var alias = Assert.Single(map).Alias;
        Assert.Equal($"{participantPart}_Ping", alias);
        Assert.Matches(ProviderLimits, alias);
    }

    [Fact]
    public void ValidFunctionNameDoesNotChange()
    {
        var map = ToolAliasBuilder.Build([Client("Bob", Function("_get_battery_level2"), Function("saveFile"))]);

        Assert.Equal(new[] { "Bob__get_battery_level2", "Bob_saveFile" }, map.Select(a => a.Alias));
    }

    [Fact]
    public void ParticipantNamesSameIgnoringCaseGetANumberAfterTheParticipant()
    {
        var first = Client("Bob", SaveFile);
        var second = Client("bob", SaveFile, Ping);
        var third = Client("BOB", Ping);
        var withoutFunctions = Client("Bob");
        var fifth = Client("Bob", Ping);

        var map = ToolAliasBuilder.Build([first, second, third, withoutFunctions, fifth]);

        // every word of a participant name starts with a capital letter, the rest keeps its case
        Assert.Equal(
            new[] { "Bob_SaveFile", "Bob2_Ping", "Bob2_SaveFile", "BOB3_Ping", "Bob5_Ping" },
            map.Select(a => a.Alias));
    }

    [Fact]
    public void SameFunctionNameFromTwoPluginsOfOneParticipantGetsANumberAfterTheFunction()
    {
        var client = ClientWithPlugins("Bob", (Guid.NewGuid(), SaveFile), (Guid.NewGuid(), SaveFile));

        var map = ToolAliasBuilder.Build([client]);

        Assert.Equal(new[] { "Bob_SaveFile", "Bob_SaveFile2" }, map.Select(a => a.Alias).Order());
        Assert.Equal(2, map.Select(a => a.FunctionId).Distinct().Count());
    }

    [Fact]
    public void TooLongAliasCutsTheParticipantPartFirst()
    {
        var longName = new string('a', 70);
        var longFunction = "F" + new string('x', 63);

        var map = ToolAliasBuilder.Build([Client(longName, SaveFile, Function(longFunction)), Client(longName, SaveFile)]);

        var aliases = map.Select(a => a.Alias).ToList();
        Assert.All(aliases, alias => Assert.Matches(ProviderLimits, alias));
        // the long function keeps 62 characters, one letter of the participant stays
        Assert.Equal("A_" + longFunction[..62], aliases[0]);
        Assert.Equal("A" + new string('a', 54) + "_SaveFile", aliases[1]);
        Assert.Equal("A" + new string('a', 53) + "2_SaveFile", aliases[2]);
    }

    [Fact]
    public void CutNamesThatRepeatGetACounterAtTheEnd()
    {
        var prefix = "F" + new string('x', 62);

        var map = ToolAliasBuilder.Build([Client("Bob", Function(prefix + "a"), Function(prefix + "b"))]);

        var aliases = map.Select(a => a.Alias).ToList();
        Assert.Equal(2, aliases.Distinct(StringComparer.OrdinalIgnoreCase).Count());
        Assert.All(aliases, alias => Assert.Matches(ProviderLimits, alias));
    }

    [Fact]
    public void AliasLeadsToItsParticipantAndFunction()
    {
        var phone = Phone;
        var laptop = Laptop;

        var map = ToolAliasBuilder.Build([phone, laptop]);

        var saveFile = map.SingleOrDefault(a => a.Alias == "BobsPhone_SaveFile");
        Assert.NotNull(saveFile);
        Assert.Equal(new ParticipantId(phone.ParticipantId), saveFile.ParticipantId);
        Assert.Equal(new FunctionId(phone.Functions.Single(f => f.Description.Name == "SaveFile").FunctionId), saveFile.FunctionId);

        var notify = map.SingleOrDefault(a => a.Alias == "AliceLaptop_Notify");
        Assert.NotNull(notify);
        Assert.Equal(new ParticipantId(laptop.ParticipantId), notify.ParticipantId);

        Assert.DoesNotContain(map, a => a.Alias == "BobsPhone_Notify");
        Assert.Equal(8, map.Count);
    }
}
