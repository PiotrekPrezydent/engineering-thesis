using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Profiles.Application.ChangeProfileName;

public record ChangeProfileNameCommand(Guid ClientId, string Name) : ICommand;