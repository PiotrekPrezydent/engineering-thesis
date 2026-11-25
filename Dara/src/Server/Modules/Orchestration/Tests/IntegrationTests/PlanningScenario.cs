using Dara.Server.Modules.Orchestration.Application.Groups.AddParticipantToGroup;
using Dara.Server.Modules.Orchestration.Application.Groups.CreateGroup;
using Dara.Server.Modules.Orchestration.Application.Groups.EditApiKey;
using Dara.Server.Modules.Orchestration.Application.Groups.EditChatClientProviderName;
using Dara.Server.Modules.Orchestration.Application.Groups.EditClientModelName;
using Dara.Server.Modules.Orchestration.Application.Participants.CreateParticipant;
using Dara.Server.Modules.Orchestration.Application.Participants.ReplacePluginFunctions;
using Dara.Server.Modules.Orchestration.Application.Participants.UpdateParticipantName;
using Dara.Server.Modules.Orchestration.Domain.AgentRuns;
using Dara.Server.Modules.Plugins.Integration;
using Dara.Shared.Contracts.Functions;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Orchestration.IntegrationTests;

// bob's phone owns a group with bob's laptop, both with plugin functions and agent settings of the owner
public class PlanningScenario
{
    public const string ApiKey = "test-api-key-not-real-1234567890";
    public const string ModelName = "gemini-test";

    public static FunctionDescription SaveFile => new("SaveFile", "Saves a file",
    [
        new PropertyDescription("path", new TypeDescription(TypeKind.String, Pattern: "^/"), true),
        new PropertyDescription("content", new TypeDescription(TypeKind.String), true)
    ], new TypeDescription(TypeKind.Boolean));

    public static FunctionDescription GetPhotosFolder => new("GetPhotosFolder", "Folder of photos", [], new TypeDescription(TypeKind.String));

    public static FunctionDescription TakePhoto => new("TakePhoto", "Takes a photo with the camera",
        [new PropertyDescription("quality", new TypeDescription(TypeKind.Integer, Minimum: 1, Maximum: 100, Default: "80"), false)],
        new TypeDescription(TypeKind.String, ContentEncoding: "base64"));

    public Guid GroupId { get; } = Guid.NewGuid();
    public Guid PhoneId { get; } = Guid.NewGuid();
    public Guid LaptopId { get; } = Guid.NewGuid();
    public Guid StrangerId { get; } = Guid.NewGuid();
    public Guid PhonePluginId { get; } = Guid.NewGuid();
    public Guid LaptopPluginId { get; } = Guid.NewGuid();

    public IReadOnlyList<AvailableFunctionSnapshot> PhoneFunctions { get; } =
        [new(Guid.NewGuid(), SaveFile), new(Guid.NewGuid(), GetPhotosFolder)];

    public IReadOnlyList<AvailableFunctionSnapshot> LaptopFunctions { get; } =
        [new(Guid.NewGuid(), TakePhoto)];

    public Guid FunctionId(string name)
    {
        return PhoneFunctions.Concat(LaptopFunctions).Single(f => f.Description.Name == name).FunctionId;
    }

    public static async Task<PlanningScenario> CreateAsync(OrchestrationModuleHost host, bool withSettings = true)
    {
        var scenario = new PlanningScenario();

        await scenario.AddParticipantAsync(host, scenario.PhoneId, "Bob-Phone", scenario.PhonePluginId, scenario.PhoneFunctions);
        await scenario.AddParticipantAsync(host, scenario.LaptopId, "Bob-Laptop", scenario.LaptopPluginId, scenario.LaptopFunctions);
        await scenario.AddParticipantAsync(host, scenario.StrangerId, "Eve-Phone", Guid.NewGuid(), [new(Guid.NewGuid(), SaveFile)]);

        await host.ExecuteAsync(new CreateGroupCommand(scenario.GroupId, scenario.PhoneId));
        await host.ExecuteAsync(new AddParticipantToGroupCommand(scenario.GroupId, scenario.LaptopId));

        if (withSettings)
        {
            await host.ExecuteAsync(new EditChatClientProviderNameCommand(scenario.GroupId, scenario.PhoneId, "google"));
            await host.ExecuteAsync(new EditClientModelNameCommand(scenario.GroupId, scenario.PhoneId, ModelName));
            await host.ExecuteAsync(new EditApiKeyCommand(scenario.GroupId, scenario.PhoneId, ApiKey));
        }

        return scenario;
    }

    public async Task<IReadOnlyList<AgentRun>> ReadRunsAsync(OrchestrationModuleHost host)
    {
        return await host.ReadAsync<IReadOnlyList<AgentRun>>(async readModel => await readModel.Query<AgentRun>()
            .Include(r => r.Plan)
            .Where(r => r.GroupId.Match(GroupId))
            .ToListAsync());
    }

    private async Task AddParticipantAsync(OrchestrationModuleHost host, Guid participantId, string name, Guid pluginId, IReadOnlyList<AvailableFunctionSnapshot> functions)
    {
        await host.ExecuteAsync(new CreateDefaultParticipantCommand(participantId));
        await host.ExecuteAsync(new UpdateParticipantNameCommand(participantId, name));
        await host.ExecuteAsync(new ReplacePluginFunctionsCommand(participantId, pluginId, 1, functions));
    }
}
