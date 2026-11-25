using Dara.Server.Modules.Orchestration.Domain.Participants;
using Dara.Server.Modules.Orchestration.Domain.Participants.Functions;
using Dara.Server.Modules.Orchestration.Domain.Participants.Functions.Descriptions;
using static Dara.Server.Modules.Orchestration.UnitTests.OrchestrationSamples;

namespace Dara.Server.Modules.Orchestration.UnitTests.Participants;

public class ParticipantTests
{
    private static readonly PluginId Files = new(Guid.NewGuid());
    private static readonly PluginId Battery = new(Guid.NewGuid());

    private static AvailableFunction Available(FunctionDescription description) => new(new FunctionId(Guid.NewGuid()), description);

    [Fact]
    public void PluginFunctionsComeWithTheirPluginAndFullDescription()
    {
        var participant = Participant.CreateDefault(new ParticipantId(Guid.NewGuid()));
        var saveFile = Available(SaveFile);

        var applied = participant.ReplacePluginFunctions(Files, 1, [saveFile, Available(ReadFile)]);

        Assert.True(applied);
        var function = participant.Functions.Single(f => f.Name == "SaveFile");
        Assert.Equal(saveFile.Id, function.Id);
        Assert.Equal(Files, function.PluginId);
        Assert.Equal(participant.Id, function.ParticipantId);
        Assert.Equal(SaveFile.Parameters, function.GetDescription().Parameters);
        Assert.Equal(SaveFile.Returns, function.GetDescription().Returns);

        var snapshot = participant.ToSnapshot();
        Assert.Equal(2, snapshot.Functions.Count);
        Assert.Equal(Files.Value, snapshot.Functions.Single(f => f.FunctionId == saveFile.Id.Value).PluginId);
    }

    [Fact]
    public void NewerVersionReplacesTheFunctionsOfItsPluginOnly()
    {
        var participant = Participant.CreateDefault(new ParticipantId(Guid.NewGuid()));
        var saveFile = Available(SaveFile);
        var readFile = Available(ReadFile);
        participant.ReplacePluginFunctions(Files, 1, [saveFile, readFile]);
        participant.ReplacePluginFunctions(Battery, 1, [Available(GetBatteryLevel)]);
        var keptInstance = participant.Functions.Single(f => f.Id == saveFile.Id);

        // read file was disabled, ping was enabled
        var applied = participant.ReplacePluginFunctions(Files, 2, [saveFile, Available(Ping)]);

        Assert.True(applied);
        Assert.Equal(new[] { "GetBatteryLevel", "Ping", "SaveFile" }, participant.Functions.Select(f => f.Name).Order());
        Assert.Same(keptInstance, participant.Functions.Single(f => f.Id == saveFile.Id));
        Assert.Equal(2, participant.PluginVersions.Single(v => v.PluginId == Files).Version);
        Assert.Equal(1, participant.PluginVersions.Single(v => v.PluginId == Battery).Version);
    }

    [Fact]
    public void OlderOrTheSameVersionIsIgnored()
    {
        var participant = Participant.CreateDefault(new ParticipantId(Guid.NewGuid()));
        participant.ReplacePluginFunctions(Files, 3, [Available(SaveFile)]);

        var older = participant.ReplacePluginFunctions(Files, 2, [Available(ReadFile)]);
        var same = participant.ReplacePluginFunctions(Files, 3, []);

        Assert.False(older);
        Assert.False(same);
        Assert.Equal("SaveFile", Assert.Single(participant.Functions).Name);
        Assert.Equal(3, Assert.Single(participant.PluginVersions).Version);
    }

    [Fact]
    public void EmptyListRemovesThePluginAndALateOlderStateDoesNotBringItBack()
    {
        var participant = Participant.CreateDefault(new ParticipantId(Guid.NewGuid()));
        participant.ReplacePluginFunctions(Files, 1, [Available(SaveFile)]);

        participant.ReplacePluginFunctions(Files, 2, []);
        var late = participant.ReplacePluginFunctions(Files, 1, [Available(SaveFile)]);

        Assert.False(late);
        Assert.Empty(participant.Functions);
        Assert.Equal(2, Assert.Single(participant.PluginVersions).Version);
    }

    [Fact]
    public void NameComesFromTheProfileLater()
    {
        var participant = Participant.CreateDefault(new ParticipantId(Guid.NewGuid()));

        participant.UpdateName("Bob's phone");

        Assert.Equal("Bob's phone", participant.ToSnapshot().Name);
    }
}
