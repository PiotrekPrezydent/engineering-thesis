using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.Extensions.AI;

namespace Dara.Server.Modules.Orchestration.IntegrationTests.Fakes;

// offline stand-in for a model: answers with the scripted rounds in order and keeps what it was sent
public class ScriptedPlanningChatClient : IChatClient
{
    private readonly Queue<ScriptedRound> _rounds;
    private readonly List<ScriptedRequest> _requests = new();
    private readonly Lock _lock = new();
    private int _callCounter;

    public ScriptedPlanningChatClient(IEnumerable<ScriptedRound> rounds)
    {
        _rounds = new Queue<ScriptedRound>(rounds);
    }

    public IReadOnlyList<ScriptedRequest> Requests
    {
        get
        {
            lock (_lock)
                return _requests.ToList();
        }
    }

    public static ScriptedRound Calls(params (string Tool, string ArgumentsJson)[] calls)
    {
        return new ScriptedRound(calls, null, null);
    }

    public static ScriptedRound Text(string text)
    {
        return new ScriptedRound([], text, null);
    }

    public static ScriptedRound Throws(Exception exception)
    {
        return new ScriptedRound([], null, exception);
    }

    public Task<ChatResponse> GetResponseAsync(IEnumerable<ChatMessage> messages, ChatOptions? options = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ScriptedRound round;
        lock (_lock)
        {
            _requests.Add(new ScriptedRequest(messages.ToList(), options?.Tools?.ToList() ?? []));
            round = _rounds.Count > 0 ? _rounds.Dequeue() : Text("The plan is complete.");
        }

        if (round.Exception is not null)
            throw round.Exception;

        var message = round.Text is not null
            ? new ChatMessage(ChatRole.Assistant, round.Text)
            : new ChatMessage(ChatRole.Assistant, round.Calls.Select(c => (AIContent)ToCall(c.Tool, c.ArgumentsJson)).ToList());

        return Task.FromResult(new ChatResponse(message)
        {
            ModelId = "scripted",
            FinishReason = round.Text is null ? ChatFinishReason.ToolCalls : ChatFinishReason.Stop
        });
    }

    public async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(IEnumerable<ChatMessage> messages, ChatOptions? options = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var response = await GetResponseAsync(messages, options, cancellationToken);
        foreach (var update in response.ToChatResponseUpdates())
            yield return update;
    }

    public object? GetService(Type serviceType, object? serviceKey = null)
    {
        return serviceKey is null && serviceType.IsInstanceOfType(this) ? this : null;
    }

    public void Dispose()
    {
    }

    // arguments as json elements, like the provider adapters give them
    private FunctionCallContent ToCall(string tool, string argumentsJson)
    {
        var arguments = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(argumentsJson)!
            .ToDictionary(a => a.Key, a => (object?)a.Value);

        return new FunctionCallContent($"call_{Interlocked.Increment(ref _callCounter)}", tool, arguments);
    }
}

public record ScriptedRound(IReadOnlyList<(string Tool, string ArgumentsJson)> Calls, string? Text, Exception? Exception);

public record ScriptedRequest(IReadOnlyList<ChatMessage> Messages, IReadOnlyList<AITool> Tools);
