using Dara.Server.Modules.Orchestration.Application.AgentRuns.GenerateExecutionPlan;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace Dara.Server.Modules.Orchestration.Infrastructure.AgentRuns.Generation;

// one prompt to the function invocation loop of microsoft.extensions.ai over the provider client, tools only record calls
public class ExecutionPlanGenerator : IExecutionPlanGenerator
{
    private const int MaxAnswerLength = 300;

    private readonly IReadOnlyList<IChatClientProvider> _providers;
    private readonly ExecutionPlanGeneratorOptions _options;
    private readonly ILogger<ExecutionPlanGenerator> _logger;

    public ExecutionPlanGenerator(IEnumerable<IChatClientProvider> providers, ExecutionPlanGeneratorOptions options, ILogger<ExecutionPlanGenerator> logger)
    {
        _providers = providers.ToList();
        _options = options;
        _logger = logger;
    }

    public async Task<ExecutionPlanGenerationResult> GenerateAsync(ExecutionPlanGenerationRequest request, IProposedCallRecorder recorder)
    {
        var provider = _providers.FirstOrDefault(p => string.Equals(p.ProviderName, request.ProviderName, StringComparison.OrdinalIgnoreCase));
        if (provider is null)
            return Failed($"unknown chat client provider '{request.ProviderName}'");

        if (string.IsNullOrWhiteSpace(request.ApiKey))
            return Failed("the group has no api key");

        if (string.IsNullOrWhiteSpace(request.ModelName))
            return Failed("the group has no model name");

        if (request.Tools.Count == 0)
            return Failed("no participant of the group has functions");

        var options = new ChatOptions
        {
            ToolMode = ChatToolMode.Auto,
            Tools = request.Tools.Select(t => (AITool)new PlanRecordingFunction(t, recorder)).ToList()
        };

        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(_options.TimeoutSeconds));
        try
        {
            // calls run one by one in the order of the answer, so result numbers follow that order
            using var client = provider.Create(request.ApiKey, request.ModelName)
                .AsBuilder()
                .UseFunctionInvocation(configure: loop =>
                {
                    loop.MaximumIterationsPerRequest = _options.MaximumIterations;
                    loop.AllowConcurrentInvocation = false;
                })
                .Build();

            var response = await client.GetResponseAsync(PlanningPrompt.CreateMessages(request), options, timeout.Token);

            return Evaluate(response);
        }
        catch (OperationCanceledException) when (timeout.IsCancellationRequested)
        {
            return Failed($"the model did not finish the plan within {_options.TimeoutSeconds} s");
        }
        catch (Exception exception)
        {
            // the provider message can repeat the request, so the key is cut out of it
            var reason = Redact($"the model provider failed: {exception.Message}", request.ApiKey);
            _logger.LogWarning("plan generation failed: {Reason}", reason);
            return Failed(reason);
        }
    }

    // results of the last round tell whether the model left a call with an error
    private static ExecutionPlanGenerationResult Evaluate(ChatResponse response)
    {
        var rounds = response.Messages
            .Select(m => m.Contents.OfType<FunctionResultContent>().Select(r => r.Result?.ToString() ?? "").ToList())
            .Where(results => results.Count > 0)
            .ToList();

        var error = rounds.LastOrDefault()?.FirstOrDefault(r => !r.StartsWith(PlanRecordingFunction.ResultPrefix, StringComparison.Ordinal));
        if (error is not null)
            return Failed($"the model left a call with an error: {Shorten(error)}");

        var accepted = rounds.SelectMany(r => r).Any(r => r.StartsWith(PlanRecordingFunction.ResultPrefix, StringComparison.Ordinal));
        if (!accepted)
            return Failed($"the model answered without a plan: {Shorten(response.Text)}");

        return new ExecutionPlanGenerationResult(true);
    }

    private static ExecutionPlanGenerationResult Failed(string reason)
    {
        return new ExecutionPlanGenerationResult(false, reason);
    }

    private static string Shorten(string text)
    {
        var trimmed = text.Trim();
        return trimmed.Length <= MaxAnswerLength ? trimmed : trimmed[..MaxAnswerLength] + "...";
    }

    private static string Redact(string text, string apiKey)
    {
        return string.IsNullOrEmpty(apiKey) ? text : text.Replace(apiKey, "***", StringComparison.Ordinal);
    }
}
