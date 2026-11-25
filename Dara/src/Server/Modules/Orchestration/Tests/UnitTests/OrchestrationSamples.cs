using Dara.Server.BuildingBlocks.Domain.Exceptions;
using Dara.Server.Modules.Orchestration.Domain.AgentRuns;
using Dara.Server.Modules.Orchestration.Domain.Groups;
using Dara.Server.Modules.Orchestration.Domain.Participants;
using Dara.Server.Modules.Orchestration.Domain.Participants.Functions;
using Dara.Server.Modules.Orchestration.Domain.Participants.Functions.Descriptions;

namespace Dara.Server.Modules.Orchestration.UnitTests;

// two devices with a few functions and a group around them; each test breaks one thing
internal static class OrchestrationSamples
{
    public static TypeDescription Text => new(TypeKind.String);

    public static TypeDescription Integer => new(TypeKind.Integer);

    public static TypeDescription Number => new(TypeKind.Number);

    public static TypeDescription Boolean => new(TypeKind.Boolean);

    public static TypeDescription ListOf(TypeDescription items) => new(TypeKind.Array, Items: items);

    public static FunctionDescription Function(string name, params PropertyDescription[] parameters)
        => new(name, $"{name} description", parameters, null);

    public static FunctionDescription SaveFile => new("SaveFile", "Saves a file",
    [
        new PropertyDescription("path", Text with { Pattern = "^/" }, true),
        new PropertyDescription("content", Text, true),
        new PropertyDescription("overwrite", Boolean with { Default = "false" }, false)
    ], Boolean);

    public static FunctionDescription ReadFile => new("ReadFile", "Reads a file",
        [new PropertyDescription("path", Text, true)], Text);

    public static FunctionDescription GetBatteryLevel => new("GetBatteryLevel", "Battery level in percent",
        [], Integer with { Minimum = 0, Maximum = 100 });

    public static FunctionDescription Ping => Function("Ping");

    public static FunctionDescription Notify => new("Notify", "Shows a notification",
    [
        new PropertyDescription("message", Text with { MaxLength = 200 }, true),
        new PropertyDescription("level", Integer with { Minimum = 1, Maximum = 3, Default = "1" }, false)
    ], null);

    public static FunctionDescription SetVolume => Function("SetVolume", new PropertyDescription("level", Number, true));

    public static FunctionDescription ListFiles => new("ListFiles", "Lists files of a folder",
        [new PropertyDescription("folder", Text, true)], ListOf(Text));

    public static FunctionDescription SendFiles => Function("SendFiles", new PropertyDescription("paths", ListOf(Text), true));

    public static ParticipantSnapshot Device(string name, params FunctionDescription[] functions)
        => DeviceWithPlugins(name, functions.Select(f => (Plugin: Guid.NewGuid(), Function: f)).ToArray());

    public static ParticipantSnapshot DeviceWithPlugins(string name, params (Guid Plugin, FunctionDescription Function)[] functions)
        => new(Guid.NewGuid(), name, functions.Select(f => new FunctionSnapshot(Guid.NewGuid(), f.Plugin, f.Function)).ToList());

    public static ParticipantSnapshot Phone => Device("Bob's phone", SaveFile, ReadFile, GetBatteryLevel, Ping);

    public static ParticipantSnapshot Laptop => Device("Alice laptop", Notify, SetVolume, ListFiles, SendFiles);

    public static Group GroupOf(params ParticipantSnapshot[] participants)
    {
        var group = Group.CreateDefault(new GroupId(Guid.NewGuid()), new ParticipantId(participants[0].ParticipantId));
        foreach (var participant in participants.Skip(1))
            group.AddParticipant(new ParticipantId(participant.ParticipantId));

        return group;
    }

    // a run of the phone owner with both devices in the group
    public static AgentRun StartRun(out ParticipantSnapshot phone, out ParticipantSnapshot laptop)
    {
        phone = Phone;
        laptop = Laptop;
        return GroupOf(phone, laptop).StartAgentRun([phone, laptop], AuthorOf(phone), Guid.NewGuid(), "save my battery level");
    }

    public static AgentRun StartRun() => StartRun(out _, out _);

    public static ParticipantId AuthorOf(ParticipantSnapshot participant) => new(participant.ParticipantId);

    public static int Propose(this AgentRun run, string alias, string argumentsJson = "{}")
        => run.ProposeCall(new ProposedCall(alias, argumentsJson));

    public static BuisnessRuleValidationException AssertBroken<TRule>(Action action)
    {
        var exception = Assert.Throws<BuisnessRuleValidationException>(action);
        Assert.IsType<TRule>(exception.BrokenRule);
        return exception;
    }
}
