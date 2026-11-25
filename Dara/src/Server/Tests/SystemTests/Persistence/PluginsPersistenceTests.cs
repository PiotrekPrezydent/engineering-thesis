using System.Text.Json;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.Modules.Plugins.Domain.PluginConfigurations;
using Dara.Server.Modules.Plugins.Domain.PluginOwners;
using Dara.Server.Modules.Plugins.Domain.Plugins;
using Dara.Server.Modules.Plugins.Domain.Plugins.Descriptions;
using Dara.Server.SystemTests.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.Server.SystemTests.Persistence;

// the plugins ef model builds and keeps the whole function description as json
public class PluginsPersistenceTests
{
    private static TypeDescription Text => new(TypeKind.String);

    private static readonly TypeDescription Address = new(TypeKind.Object, Nullable: true, Description: "Where to send", Properties:
    [
        new("street", Text, true),
        new("zip", Text with { Nullable = true, Pattern = "^[0-9-]+$" }, false)
    ]);

    private static readonly FunctionDescription SendLetter = new("SendLetter", "Sends a letter",
    [
        new("to", Address, true),
        new("pages", new TypeDescription(TypeKind.Array, MinItems: 1, Items: Text with { MaxLength = 2000 }), true),
        new("priority", Text with { Enum = ["Low", "High"], Default = "\"Low\"" }, false),
        new("copies", new TypeDescription(TypeKind.Integer, Minimum: 1, Maximum: 5.5m, Default: "1"), false),
        new("meta", new TypeDescription(TypeKind.Object, AdditionalProperties: new TypeDescription(TypeKind.Any)), false)
    ], new TypeDescription(TypeKind.Array, Items: Address with { Nullable = false }));

    private static readonly FunctionDescription Ping = new("Ping", null, [], null);

    [Fact]
    public async Task PluginWithFullDescriptionAndItsConfigurationAreStoredAndRead()
    {
        await using var host = await ModulesHost.StartAsync(TestContext.Current.CancellationToken);
        var ownerId = new PluginOwnerId(Guid.NewGuid());

        PluginId pluginId;
        await using (var scope = host.PluginsRoot.CreateAsyncScope())
        {
            var owner = PluginOwner.CreateDefault(ownerId);
            var plugin = owner.AddPlugin([], "Post", null, "2.1.0", new PluginHash(new string('F', 64)), [SendLetter, Ping]);
            pluginId = plugin.Id;

            await scope.ServiceProvider.GetRequiredService<IPluginOwnerRepository>().AddAsync(owner);
            await scope.ServiceProvider.GetRequiredService<IPluginRepository>().AddAsync(plugin);
            await scope.ServiceProvider.GetRequiredService<IUnitOfWork>().CommitAsync();
        }

        await using (var scope = host.PluginsRoot.CreateAsyncScope())
        {
            var stored = Assert.Single(await scope.ServiceProvider.GetRequiredService<IPluginRepository>().GetByOwnerIdAsync(ownerId));

            Assert.Equal(pluginId, stored.Id);
            Assert.Equal(new string('f', 64), stored.Hash.Value);
            Assert.Null(stored.Description);
            Assert.Equal(Json(SendLetter), Json(stored.Functions.Single(f => f.Name == "SendLetter").GetDescription()));
            Assert.Equal(Json(Ping), Json(stored.Functions.Single(f => f.Name == "Ping").GetDescription()));

            var configuration = await scope.ServiceProvider.GetRequiredService<IPluginConfigurationRepository>().GetByPluginIdAsync(pluginId);
            Assert.Equal(ownerId, configuration.OwnerId);
            Assert.Equal(1, configuration.Version);
            Assert.Equal(stored.Functions.Select(f => f.Id.Value).Order(), configuration.GetAvailableFunctionIds().Select(id => id.Value).Order());
        }
    }

    private static string Json(FunctionDescription description) => JsonSerializer.Serialize(description);
}
