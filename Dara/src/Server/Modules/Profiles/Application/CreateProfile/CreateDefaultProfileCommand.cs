using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Profiles.Application.CreateProfile;

public record CreateDefaultProfileCommand(Guid ProfileId) : ICommand;