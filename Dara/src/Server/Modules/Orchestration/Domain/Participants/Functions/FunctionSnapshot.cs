using Dara.Server.Modules.Orchestration.Domain.Participants.Functions.Descriptions;

namespace Dara.Server.Modules.Orchestration.Domain.Participants.Functions;

public record FunctionSnapshot(Guid FunctionId, Guid PluginId, FunctionDescription Description);
