using System.Text.Json.Serialization;
using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Orchestration.Application.AgentRuns.StartAgentRun;

public record StartAgentRunCommand(Guid MessageId, Guid GroupId, Guid AuthorId, string Message) : IOrderedInternalCommand
{
    // queued commands of one group run in order
    [JsonIgnore]
    public string OrderingKey => $"group:{GroupId}";
}
