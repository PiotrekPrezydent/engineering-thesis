using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Plugins.Domain.Plugins;

// sha-256 of the plugin .dll counted by the client, the natural key of a plugin within its owner
public record PluginHash(string Value) : IValueObject;
