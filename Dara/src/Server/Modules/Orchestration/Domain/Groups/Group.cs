using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Orchestration.Domain.AgentRuns;
using Dara.Server.Modules.Orchestration.Domain.Groups.Rules;
using Dara.Server.Modules.Orchestration.Domain.Participants;

namespace Dara.Server.Modules.Orchestration.Domain.Groups;

// members of a group copied from the groups module and the agent settings of the group, which only orchestration has
public class Group : Entity, IAggregateRoot
{
    public GroupId Id { get; private set; }

    public ParticipantId OwnerId { get; private set; }

    public IReadOnlyList<ParticipantId> Participants => _participants.AsReadOnly();
    
    private List<ParticipantId> _participants;
    
    public GroupAgentSettings AgentSettings { get; private set; }

    // only the owner reads and changes it, it is never part of a snapshot or an event
    public string ApiKey { get; private set; }
    
    private Group()
    {
    }

    private Group(GroupId id, ParticipantId owner, GroupAgentSettings agentSettings)
    {
        Id = id;
        OwnerId = owner;
        AgentSettings = agentSettings;
        ApiKey = "";
        
        _participants = new List<ParticipantId> { owner };
    }

    public static Group CreateDefault(GroupId id, ParticipantId owner)
    {
        return new Group(id, owner, new GroupAgentSettings("/prompt", "", ""));
    }

    // integration events can come twice, so adding a present and removing a missing participant change nothing
    public void AddParticipant(ParticipantId participant)
    {
        if (!_participants.Contains(participant))
            _participants.Add(participant);
    }
    
    public void RemoveParticipant(ParticipantId participant)
    {
        _participants.Remove(participant);
    }

    public void EditPromptMessagePrefix(ParticipantId editorId, string promptMessagePrefix)
    {
        CheckRule(new OnlyGroupOwnerCanChangeAgentSettingsRule(OwnerId, editorId));
        CheckRule(new PromptMessagePrefixMustBeProvidedRule(promptMessagePrefix));
        AgentSettings = AgentSettings with { PromptMessagePrefix = promptMessagePrefix };
    }
    
    public void EditChatClientProviderName(ParticipantId editorId, string chatClientProviderName)
    {
        CheckRule(new OnlyGroupOwnerCanChangeAgentSettingsRule(OwnerId, editorId));
        AgentSettings = AgentSettings with { ChatClientProviderName = chatClientProviderName };
    }

    public void EditClientModelName(ParticipantId editorId, string clientModelName)
    {
        CheckRule(new OnlyGroupOwnerCanChangeAgentSettingsRule(OwnerId, editorId));
        AgentSettings = AgentSettings with { ClientModelName = clientModelName };
    }

    public void EditApiKey(ParticipantId editorId, string apiKey)
    {
        CheckRule(new OnlyGroupOwnerCanChangeAgentSettingsRule(OwnerId, editorId));
        ApiKey = apiKey;
    }

    // text after the prefix and white space, null for an ordinary message
    public string? FindPrompt(string message)
    {
        var prefix = AgentSettings.PromptMessagePrefix;
        if (!message.StartsWith(prefix, StringComparison.Ordinal) || message.Length == prefix.Length
                                                                   || !char.IsWhiteSpace(message[prefix.Length]))
            return null;

        var prompt = message[prefix.Length..].Trim();
        return prompt.Length > 0 ? prompt : null;
    }

    // the run keeps snapshots of the group and of the participants as the model sees them
    public AgentRun StartAgentRun(IReadOnlyList<ParticipantSnapshot> participants, ParticipantId authorId, Guid messageId, string prompt)
    {
        CheckRule(new AuthorMustBeGroupParticipantRule(Participants, authorId));
        CheckRule(new PromptMustBeProvidedRule(prompt));

        return AgentRun.Start(ToSnapshot(), participants, authorId, messageId, prompt);
    }

    public GroupSnapshot ToSnapshot()
    {
        return new GroupSnapshot(
            Id.Value,
            OwnerId.Value,
            AgentSettings.PromptMessagePrefix,
            AgentSettings.ChatClientProviderName,
            AgentSettings.ClientModelName,
            _participants.Select(p => p.Value).ToList());
    }
}