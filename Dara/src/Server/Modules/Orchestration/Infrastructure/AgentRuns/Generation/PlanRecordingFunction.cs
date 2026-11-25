using System.Text.Json;
using Dara.Server.Modules.Orchestration.Application.AgentRuns.GenerateExecutionPlan;
using Dara.Shared.Contracts.Functions;
using Microsoft.Extensions.AI;

namespace Dara.Server.Modules.Orchestration.Infrastructure.AgentRuns.Generation;

// a tool the model sees for one participant function; a call is added to the plan instead of running and returns {{result:N}}
public class PlanRecordingFunction : AIFunction
{
    public const string ResultPrefix = "{{result:";
    public const string ErrorPrefix = "Error: ";

    private readonly PlanningTool _tool;
    private readonly IProposedCallRecorder _recorder;
    private readonly JsonElement _jsonSchema;
    private readonly JsonElement? _returnJsonSchema;

    public PlanRecordingFunction(PlanningTool tool, IProposedCallRecorder recorder)
    {
        _tool = tool;
        _recorder = recorder;
        _jsonSchema = FunctionJsonSchema.CreateParametersSchema(tool.Function);
        _returnJsonSchema = FunctionJsonSchema.CreateReturnSchema(tool.Function);
    }

    public override string Name => _tool.Alias;

    public override string Description => string.IsNullOrWhiteSpace(_tool.Function.Description)
        ? $"Function of the client {_tool.ClientName}."
        : $"{_tool.Function.Description} (function of the client {_tool.ClientName})";

    public override JsonElement JsonSchema => _jsonSchema;

    public override JsonElement? ReturnJsonSchema => _returnJsonSchema;

    // an error goes back as text, an exception would end the loop after three rounds
    protected override ValueTask<object?> InvokeCoreAsync(AIFunctionArguments arguments, CancellationToken cancellationToken)
    {
        var argumentsJson = JsonSerializer.Serialize<IDictionary<string, object?>>(arguments, AIJsonUtilities.DefaultOptions);
        var result = _recorder.Record(_tool.Alias, argumentsJson);

        object? answer = result.Index is { } index
            ? ResultPrefix + index + "}}"
            : ErrorPrefix + result.Error;

        return ValueTask.FromResult<object?>(answer);
    }
}
