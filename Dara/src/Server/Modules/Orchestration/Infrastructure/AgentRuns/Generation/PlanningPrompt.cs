using Dara.Server.Modules.Orchestration.Application.AgentRuns.GenerateExecutionPlan;
using Microsoft.Extensions.AI;

namespace Dara.Server.Modules.Orchestration.Infrastructure.AgentRuns.Generation;

// the system prompt of the owner's planning test with result placeholders of the plan
public static class PlanningPrompt
{
    public const string SystemPromptFormat =
        """
        You are an orchestration agent controlling multiple clients in a group that are connected to an RPC server.
        Your goal is to complete the request specified by the current group message using tool calls.
        The server does not run the tools now: every tool call adds one step to a plan that the author of the message accepts or rejects as a whole, the server dispatches the calls after the plan is accepted.
        The result of each tool call is a placeholder {{result:N}}, where N is the number of the call in the plan. You may pass the placeholder to a later tool call as the whole argument value or inside a text when that result is needed.
        If a tool call returns an error, correct the call and call the tool again.

        Client and group member naming is used alternately, but they represent the same instance of connection to the server.
        Each tool belongs to a specific client, the tool name starts with the client name without spaces and special characters: [CLIENT]_[TOOL-NAME], and the tool description names the client.
        Each user chat message has a known format specifying the client name and the message: [CLIENT-NAME] : [MESSAGE]

        Available clients in the group and their tools:
        {clients}

        Rules:
        1. Always choose tools according to the client they belong to.
        2. Never assume that two clients have the same state, files, directories, or capabilities.
        3. When the group member asks to perform an action involving multiple members, use the appropriate tool for each member.
        4. A tool call must target the client that owns that capability.
        5. You may use the result of a previous tool call as input for a later tool call.
        6. Do not invent tools or capabilities that are not available.
        7. Prefer the client explicitly mentioned by the group member.
        8. Every tool argument must come from:
            - the user's request,
            - available group/client context,
            - or the result of a previous tool call.
        9. If a required argument cannot be obtained from these sources, do not call the tool and pass a message indicating the missing data.

        When the plan is complete, stop making tool calls and provide a concise summary of the planned actions.
        """;

    public static List<ChatMessage> CreateMessages(ExecutionPlanGenerationRequest request)
    {
        var clients = request.ClientNames
            .Select(name => $"- {name}: {ToolsOf(request, name)}");

        var systemPrompt = SystemPromptFormat.Replace("{clients}", string.Join("\n", clients));

        return
        [
            new ChatMessage(ChatRole.System, systemPrompt),
            new ChatMessage(ChatRole.User, $"{request.AuthorName} : {request.Prompt}")
        ];
    }

    private static string ToolsOf(ExecutionPlanGenerationRequest request, string clientName)
    {
        var aliases = request.Tools.Where(t => t.ClientName == clientName).Select(t => t.Alias).ToList();
        return aliases.Count == 0 ? "no tools" : string.Join(", ", aliases);
    }
}
