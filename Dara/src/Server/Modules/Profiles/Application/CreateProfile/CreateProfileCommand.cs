using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Profiles.Application.CreateProfile;

public record CreateProfileCommand(Guid ProfileId, string Name) : ICommand;