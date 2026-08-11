using System.Collections.Immutable;
using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Plugins.Domain.PluginOwners.Plugins;

public record PluginFunction(string Name, string Description, string ReturnType, ImmutableArray<PluginFunctionParameter> Parameters) : IValueObject;