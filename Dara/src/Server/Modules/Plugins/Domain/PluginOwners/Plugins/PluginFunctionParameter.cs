using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Plugins.Domain.PluginOwners.Plugins;

public record PluginFunctionParameter(string Name, string Description, string Type) : IValueObject;