using Dara.Server.Modules.Orchestration.Domain.Groups;
using Dara.Server.Modules.Orchestration.Domain.Groups.Rules;
using Dara.Server.Modules.Orchestration.Domain.Participants;
using static Dara.Server.Modules.Orchestration.UnitTests.OrchestrationSamples;

namespace Dara.Server.Modules.Orchestration.UnitTests.Groups;

public class GroupTests
{
    private static readonly ParticipantId Owner = new(Guid.NewGuid());
    private static readonly ParticipantId Member = new(Guid.NewGuid());

    private static Group NewGroup() => Group.CreateDefault(new GroupId(Guid.NewGuid()), Owner);

    [Fact]
    public void NewGroupHasItsOwnerAsParticipantAndDefaultSettings()
    {
        var group = NewGroup();

        Assert.Equal(Owner, group.OwnerId);
        Assert.Equal(new[] { Owner }, group.Participants);
        Assert.Equal(new GroupAgentSettings("/prompt", "", ""), group.AgentSettings);
        Assert.Equal("", group.ApiKey);
    }

    [Fact]
    public void AddingTwiceAndRemovingAMissingParticipantChangeNothing()
    {
        var group = NewGroup();

        group.AddParticipant(Member);
        group.AddParticipant(Member);
        Assert.Equal(new[] { Owner, Member }, group.Participants);

        group.RemoveParticipant(Member);
        group.RemoveParticipant(Member);
        Assert.Equal(new[] { Owner }, group.Participants);
    }

    [Fact]
    public void OwnerChangesTheAgentSettings()
    {
        var group = NewGroup();

        group.EditPromptMessagePrefix(Owner, "!ai");
        group.EditChatClientProviderName(Owner, "google");
        group.EditClientModelName(Owner, "gemini-2.5-flash");
        group.EditApiKey(Owner, "secret-key");

        Assert.Equal(new GroupAgentSettings("!ai", "google", "gemini-2.5-flash"), group.AgentSettings);
        Assert.Equal("secret-key", group.ApiKey);
    }

    [Fact]
    public void OnlyOwnerChangesTheAgentSettings()
    {
        var group = NewGroup();
        group.AddParticipant(Member);

        AssertBroken<OnlyGroupOwnerCanChangeAgentSettingsRule>(() => group.EditPromptMessagePrefix(Member, "!ai"));
        AssertBroken<OnlyGroupOwnerCanChangeAgentSettingsRule>(() => group.EditChatClientProviderName(Member, "google"));
        AssertBroken<OnlyGroupOwnerCanChangeAgentSettingsRule>(() => group.EditClientModelName(Member, "model"));
        AssertBroken<OnlyGroupOwnerCanChangeAgentSettingsRule>(() => group.EditApiKey(Member, "key"));
        Assert.Equal(new GroupAgentSettings("/prompt", "", ""), group.AgentSettings);
        Assert.Equal("", group.ApiKey);
    }

    [Theory]
    [InlineData("")]
    [InlineData("/ai now")]
    public void PromptPrefixMustBeOneWord(string prefix)
    {
        AssertBroken<PromptMessagePrefixMustBeProvidedRule>(() => NewGroup().EditPromptMessagePrefix(Owner, prefix));
    }

    [Theory]
    [InlineData("/prompt turn on the light", "turn on the light")]
    [InlineData("/prompt\n  two lines  ", "two lines")]
    [InlineData("/promptx turn on the light", null)]
    [InlineData("/prompt", null)]
    [InlineData("/prompt    ", null)]
    [InlineData("hello /prompt", null)]
    [InlineData("/PROMPT hello", null)]
    public void PromptIsTheTextAfterThePrefix(string message, string? prompt)
    {
        Assert.Equal(prompt, NewGroup().FindPrompt(message));
    }

    [Fact]
    public void ChangedPrefixIsUsed()
    {
        var group = NewGroup();
        group.EditPromptMessagePrefix(Owner, "!ai");

        Assert.Equal("hello", group.FindPrompt("!ai hello"));
        Assert.Null(group.FindPrompt("/prompt hello"));
    }

    [Fact]
    public void ApiKeyStaysOutOfTheSnapshotAndOfToString()
    {
        var group = NewGroup();
        group.EditApiKey(Owner, "secret-key");

        var snapshot = group.ToSnapshot();

        Assert.DoesNotContain(typeof(GroupSnapshot).GetProperties(), p => p.Name.Contains("Key"));
        Assert.DoesNotContain("secret-key", snapshot.ToString());
        Assert.DoesNotContain("secret-key", group.AgentSettings.ToString());
        Assert.Equal(Owner.Value, snapshot.OwnerId);
        Assert.Equal(new[] { Owner.Value }, snapshot.ParticipantsIds);
    }
}
