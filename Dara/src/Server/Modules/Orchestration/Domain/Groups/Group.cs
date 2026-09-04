using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Orchestration.Domain.Participants;

namespace Dara.Server.Modules.Orchestration.Domain.Groups;

public class Group : Entity, IAggregateRoot
{
    public GroupId Id { get; private set; }

    public IReadOnlyList<ParticipantId> Participants => _participants.AsReadOnly();
    
    private List<ParticipantId> _participants;
    
    public string PromptMessagePrefix { get; private set; }
    public string ChatClientProviderName { get; private set; }
    public string ClientModelName { get; private set; }
    public string ApiKey { get; private set; }
    
    private Group()
    {
    }

    private Group(GroupId id, ParticipantId creator,string promptMessagePrefix, string chatClientProviderName, string clientModelName, string apiKey)
    {
        Id = id;
        
        PromptMessagePrefix = promptMessagePrefix;
        ChatClientProviderName = chatClientProviderName;
        ClientModelName = clientModelName;
        ApiKey = apiKey;
        
        _participants = new List<ParticipantId> { creator };
    }

    public static Group CreateDefault(GroupId id, ParticipantId creator)
    {
        return new Group(id, creator,"/prompt","","","");
    }

    public void AddParticipant(ParticipantId participant)
    {
        _participants.Add(participant);
    }
    
    public void RemoveParticipant(ParticipantId participant)
    {
        _participants.Remove(participant);
    }

    public void EditPromptMessagePrefix(string promptMessagePrefix)
    {
        PromptMessagePrefix = promptMessagePrefix;
    }
    
    public void EditChatClientProviderName(string chatClientProviderName)
    {
        ChatClientProviderName = chatClientProviderName;
    }

    public void EditClientModelName(string clientModelName)
    {
        ClientModelName = clientModelName;
    }

    public void EditApiKey(string apiKey)
    {
        ApiKey = apiKey;
    }
}