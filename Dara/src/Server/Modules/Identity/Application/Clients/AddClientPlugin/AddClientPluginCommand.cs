using Dara.Server.BuildingBlocks.Application;

namespace Dara.Server.Modules.Identity.Application.Clients.AddClientPlugin;

public record AddClientPluginCommand(Guid ClientId, string SourceType, string RawData) : IModuleCommand;