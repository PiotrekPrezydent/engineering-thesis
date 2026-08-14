using Dara.Server.Modules.Orchestration.Domain.Participants.Functions;

namespace Dara.Server.Modules.Orchestration.Application.Participants;

public record FunctionData(Guid Id, string Name, string Description,  string ReturnTypeName, IReadOnlyList<FunctionParameterData> Parameters);

public record FunctionParameterData(string Name, string Description, string TypeName);