using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Orchestration.Domain.AgentRuns.ExecutionPlans;
using Dara.Server.Modules.Orchestration.Domain.Participants.Functions.Descriptions;

namespace Dara.Server.Modules.Orchestration.Domain.AgentRuns.Rules;

// a result passed on must fit the parameter; a result inside a text becomes text, so any result except void fits
public class ResultTypeMustMatchParameterRule : IBuisnessRule
{
    private readonly FunctionDescription _function;
    private readonly IReadOnlyList<CallArgument> _arguments;
    private readonly IReadOnlyDictionary<int, TypeDescription?> _earlierResults;
    private string? _problem;

    // earlier results: index of an earlier call and what its function returns, null for void
    public ResultTypeMustMatchParameterRule(FunctionDescription function, IReadOnlyList<CallArgument> arguments,
        IReadOnlyDictionary<int, TypeDescription?> earlierResults)
    {
        _function = function;
        _arguments = arguments;
        _earlierResults = earlierResults;
    }

    public string Message => $"Arguments of {_function.Name} are not valid: {_problem ?? FindProblem()}";
    public bool IsBroken()
    {
        _problem = FindProblem();
        return _problem is not null;
    }

    private string? FindProblem()
    {
        foreach (var argument in _arguments.Where(a => a.Kind != CallArgumentKind.Value))
        {
            foreach (var index in argument.ResultIndexes)
            {
                if (!_earlierResults.TryGetValue(index, out var result))
                    continue;

                if (result is null)
                    return $"{argument.Name}: call {index} returns nothing, so {{{{result:{index}}}}} has no value";

                var parameter = _function.Parameters.FirstOrDefault(p => p.Name == argument.Name);
                if (argument.Kind == CallArgumentKind.Result && parameter is not null && !Accepts(parameter.Type, result))
                    return $"{argument.Name}: call {index} returns {Describe(result)}, the argument needs {Describe(parameter.Type)}";
            }
        }

        return null;
    }

    // nullability is not compared, a null result fails on the executing device like any other call error
    private static bool Accepts(TypeDescription parameter, TypeDescription result)
    {
        if (parameter.Kind == TypeKind.Any || result.Kind == TypeKind.Any)
            return true;

        if (parameter.Kind == TypeKind.Number && result.Kind == TypeKind.Integer)
            return true;

        if (parameter.Kind != result.Kind)
            return false;

        if (parameter.Kind == TypeKind.Array)
            return parameter.Items is null || result.Items is null || Accepts(parameter.Items, result.Items);

        if (parameter.Kind != TypeKind.Object)
            return true;

        if (parameter.AdditionalProperties is not null && result.AdditionalProperties is not null
                                                       && !Accepts(parameter.AdditionalProperties, result.AdditionalProperties))
            return false;

        // a result without known properties may still have them
        if (parameter.Properties is null || result.Properties is null)
            return true;

        return parameter.Properties.Where(p => p.Required).All(p =>
            result.Properties.FirstOrDefault(r => r.Name == p.Name) is { } property && Accepts(p.Type, property.Type));
    }

    private static string Describe(TypeDescription type)
    {
        var kind = type.Kind.ToString().ToLowerInvariant();
        return type.Kind == TypeKind.Array && type.Items is not null ? $"{kind} of {Describe(type.Items)}" : kind;
    }
}
